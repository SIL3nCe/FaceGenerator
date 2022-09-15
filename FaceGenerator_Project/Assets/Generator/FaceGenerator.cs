using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using TMPro;

public struct SName
{
    public string FirstName;
    public string LastName;
}

// Class to serialize face part data in json format
public class FacePartData
{
    public List<int> PartPercentageHeightList = new();
}

public class FaceGenerator : MonoBehaviour
{
    // Get total generated image height based on smallest gathered image
    private int _totalImageHeight = -1;
    public int TotalImageHeight => _totalImageHeight;

    // Face part data
    private readonly string _facePartSettingsFilePath = "./FacePartSettings.json";
    public GameObject FacePartPrefab;
    private List<FacePart> _faceParts = new();

    // List of gathered images
    private readonly string _imageDirectoryPath = "./Images/";
    private List<Texture2D> _imageList = new();

    // Names gathered from images
    private List<SName> _namesList = new();

    // Face Name UI
    public TMP_Text NameText;
    public GameObject NameUIPrefab;
    private GameObject _nameUI;
    private bool _nameLocked = false;

    // Static Instance
    private static FaceGenerator _instance = null;
    public static FaceGenerator Instance => _instance;

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        // Load images
        if (Directory.Exists(_imageDirectoryPath))
        {
            DirectoryInfo dirInfo = new(_imageDirectoryPath);
            foreach (FileInfo file in dirInfo.GetFiles())
            {
                CreateTextureFromFilePath(file.FullName);

                // Extract last/first name from image name
                string nameWithoutExt = file.Name.Replace(file.Extension, "");
                string[] nameSplit = nameWithoutExt.Split("_");
                if (nameSplit.Length > 1)
                {
                    SName name = new();
                    name.FirstName = nameSplit[0];
                    name.LastName = nameSplit[1];
                    _namesList.Add(name);
                }
            }

            // Adjust location to keep it centered to the left, Initially configured for 512x512
            Vector2 pos = transform.position;
            pos.x *= _totalImageHeight / 512.0f;
            transform.position = pos;

            // Adjust camera zoom based on total height
            Camera.main.orthographicSize = _totalImageHeight * 0.008f;

            // Load face part settings and create them
            if (File.Exists(_facePartSettingsFilePath))
            {
                string facePartSettings = File.ReadAllText(_facePartSettingsFilePath);
                FacePartData facePartData = JsonUtility.FromJson<FacePartData>(facePartSettings);

                for (int i = 0; i < facePartData.PartPercentageHeightList.Count; ++i)
                {
                    FacePart facePart = Instantiate(FacePartPrefab, transform).GetComponent<FacePart>();
                    facePart.Initialize(i, facePartData.PartPercentageHeightList[i]);
                    _faceParts.Add(facePart);

                    FacePartSettingsUI.Instance.CreatePartSlider(facePart.PartPixelHeightPercentage);
                }
            }

            // Create Name buttons UI
            if (_namesList.Count > 0)
            {
                _nameUI = Instantiate(NameUIPrefab);
                pos = _nameUI.transform.position;
                pos.x = _totalImageHeight * 0.005f;
                pos.y = Camera.main.ScreenToWorldPoint(NameText.transform.position).y;
                _nameUI.transform.position = pos;
            }

            // Update last part pixel height before starting since BaseImageHeight has just been computed here
            SetupPartPixelHeights();

            // Start generation
            GenerateFace();
        }
    }

    public void SaveCurrentPartHeights()
    {
        FacePartData partData = new();
        foreach (FacePart part in _faceParts)
        {
            partData.PartPercentageHeightList.Add(part.PartPixelHeightPercentage);
        }
        string jsonData = JsonUtility.ToJson(partData);
        File.WriteAllText(_facePartSettingsFilePath, jsonData);
    }

    private void CreateTextureFromFilePath(string filePath)
    {
        byte[] bytes = System.IO.File.ReadAllBytes(filePath);

        Texture2D tex = new(0, 0);
        tex.LoadImage(bytes);

        _totalImageHeight = _totalImageHeight == -1 ? tex.height : Mathf.Min(_totalImageHeight, tex.height);

        _imageList.Add(tex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GenerateFace();
        }
    }

    public void GenerateFace()
    {
        if (_imageList.Count == 0)
            return;

        // Get random image for every part and crop them
        foreach (FacePart facePart in _faceParts)
        {
            facePart.RandomizePart(_imageList);
        }

        AdjustPartLocation();

        RandomizeName();
    }

    private void AdjustPartLocation()
    {
        for (int i = 0; i < _faceParts.Count; ++i)
        {
            _faceParts[i].SetPositionBasedOnPixelHeightAccumulation(i == 0 ? null : _faceParts[i - 1]);
        }

        // Center generated image on y 0
        Vector2 pos = transform.position;
        pos.y += _totalImageHeight * 0.005f;
        transform.position = pos;
    }

    public void CreatePart()
    {
        // Avoid creating parts if we don't have images
        if (_imageList.Count == 0)
            return;

        FacePart facePart = Instantiate(FacePartPrefab, transform).GetComponent<FacePart>();
        facePart.Initialize(_faceParts.Count, 20);
        _faceParts.Add(facePart);

        FacePartSettingsUI.Instance.CreatePartSlider(facePart.PartPixelHeightPercentage);

        facePart.RandomizePart(_imageList);

        // Update every part and recompute last part percentage
        OnPartSliderUpdated(facePart.FacePartID, 20);
    }

    public void RemoveLastPart()
    {
        Destroy(_faceParts[^1].gameObject);
        _faceParts.RemoveAt(_faceParts.Count - 1);

        // Update every part and recompute last part percentage
        OnPartSliderUpdated(_faceParts.Count - 1, 100);
    }

    public void OnPartSliderUpdated(int partID, int newPixelHeight)
    {
        if (_faceParts.Count == 0)
            return;

        _faceParts[partID].PartPixelHeightPercentage = newPixelHeight;

        SetupPartPixelHeights();
        AdjustPartLocation();

        foreach (FacePart facePart in _faceParts)
        {
            facePart.ApplyCurrentFaceID(_imageList);
        }
    }

    public bool LockName()
    {
        _nameLocked = !_nameLocked;
        return _nameLocked;
    }

    public void RandomizeName()
    {
        if (_nameLocked
            || _namesList.Count == 0
            || _faceParts.Count == 0)
        {
            _nameUI.SetActive(false);
            return;
        }

        _nameUI.SetActive(true);

        // Get random name in the selected faces and invert first letters
        int randId = _faceParts[UnityEngine.Random.Range(0, _faceParts.Count)].CurrentfaceID;
        randId = Mathf.Min(randId, _namesList.Count - 1);
        string firstName = _namesList[randId].FirstName;

        randId = _faceParts[UnityEngine.Random.Range(0, _faceParts.Count)].CurrentfaceID;
        randId = Mathf.Min(randId, _namesList.Count - 1);
        string lastName = _namesList[randId].LastName;

        SName randName = new();
        randName.FirstName = lastName[0].ToString().ToUpper() + firstName.Remove(0, 1);
        randName.LastName = firstName[0].ToString().ToUpper() + lastName.Remove(0, 1);

        NameText.text = randName.FirstName + " " + randName.LastName;
    }

    private void SetupPartPixelHeights()
    {
        if (_faceParts.Count == 0)
            return;

        // Check if percentage are valid and set to 0 if not
        int accPerc = 0;
        for (int i = 0; i < _faceParts.Count - 1; ++i)
        {
            _faceParts[i].PartPixelHeightPercentage = Mathf.Clamp(_faceParts[i].PartPixelHeightPercentage, 0, 100 - accPerc);
            accPerc += _faceParts[i].PartPixelHeightPercentage;
            accPerc = Mathf.Min(accPerc, 100);

            FacePartSettingsUI.Instance.SetPartSliderValue(i, _faceParts[i].PartPixelHeightPercentage);
        }

        // Bottom integrate all the remaining pixel percentage
        _faceParts[^1].PartPixelHeightPercentage = 100 - accPerc;
        FacePartSettingsUI.Instance.SetPartSliderValue(_faceParts.Count - 1, _faceParts[^1].PartPixelHeightPercentage);

        // Generate pixel height based on percentages
        float accVal = 0;
        for (int i = 0; i < _faceParts.Count - 1; ++i)
        {
            float pixelHeight = _totalImageHeight * (_faceParts[i].PartPixelHeightPercentage * 0.01f);
            _faceParts[i].PartPixelHeight = (int)pixelHeight;
            accVal += pixelHeight;
            _faceParts[i].PixelHeightAccumulation = (int)accVal;
        }

        // Bottom integrate all the remaining pixel height
        _faceParts[^1].PartPixelHeight = _totalImageHeight - (int)accVal;
        _faceParts[^1].PixelHeightAccumulation = _totalImageHeight;

        // Update UI slider of the part with potentially updated percentage
    }

    public void LoadPreviousPart(int partID)
    {
        if (_faceParts[partID].IsLocked)
            return;

        _faceParts[partID].LoadNextTextureID(_imageList, false);
        RandomizeName();
    }
    
    public void LoadNextPart(int partID)
    {
        if (_faceParts[partID].IsLocked)
            return;

        _faceParts[partID].LoadNextTextureID(_imageList, true);
        RandomizeName();
    }

    public bool LockPart(int partID)
    {
        _faceParts[partID].IsLocked = !_faceParts[partID].IsLocked;
        return _faceParts[partID].IsLocked;
    }

    public void RandomizePart(int partID)
    {
        if (_faceParts[partID].IsLocked)
            return;

        _faceParts[partID].RandomizePart(_imageList);
        RandomizeName();
    }

    public void ReversePart(int partID)
    {
        _faceParts[partID].ReversePart();
    }
}
