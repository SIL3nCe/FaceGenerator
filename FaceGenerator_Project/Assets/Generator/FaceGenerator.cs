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

public class FaceGenerator : MonoBehaviour
{
    // Get total generated image height based on smallest gathered image
    private int _totalImageHeight = -1;

    // Face part data
    public List<FacePart> FaceParts = new();

    // List of gathered images
    private readonly string _imageDirectoryPath = "./Images/";
    private List<Texture2D> _imageList = new();

    // Names gathered from images
    private List<SName> _namesList = new();

    public TMP_Text NameText;

    void Start()
    {
        if (Directory.Exists(_imageDirectoryPath))
        {
            DirectoryInfo dirInfo = new(_imageDirectoryPath);
            foreach (FileInfo file in dirInfo.GetFiles())
            {
                CreateTextureFromFilePath(file.FullName);

                string nameWithoutExt = file.Name.Replace(file.Extension, "");
                string[] nameSplit = nameWithoutExt.Split("_");
                if (nameSplit.Length >= 1)
                {
                    SName name = new();
                    name.FirstName = nameSplit[0];
                    name.LastName = nameSplit[1];
                    _namesList.Add(name);
                }
            }

            // Adjust camera zoom
            Camera.main.orthographicSize = _totalImageHeight * 0.01f;

            // Update last part pixel height before starting since BaseImageHeight has just been computed here
            SetLastPartPixelHeight();

            // Compute pixel height accumulation for every part before generating
            int pixelHeightAccumulation = 0;
            foreach (FacePart facePart in FaceParts)
            {
                pixelHeightAccumulation += facePart.PartPixelHeight;
                facePart.PixelHeightAccumulation = pixelHeightAccumulation;
            }

            // Start generation
            GenerateFace();
        }
    }

    private void CreateTextureFromFilePath(string filePath)
    {
        byte[] bytes = System.IO.File.ReadAllBytes(filePath);

        Texture2D tex = new Texture2D(0, 0);
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
        foreach (FacePart facePart in FaceParts)
        {
            facePart.RandomizePart(_imageList);
        }

        AdjustPartLocation();

        // Center generated image on y 0
        Vector2 pos = transform.position;
        pos.y += _totalImageHeight * 0.005f;
        transform.position = pos;

        RandomizeName();
    }

    public void AdjustPartLocation()
    {
        for (int i = 0; i < FaceParts.Count; ++i)
        {
            FaceParts[i].SetPositionBasedOnPixelHeightAccumulation(i == 0 ? null : FaceParts[i - 1]);
        }
    }

    private void RandomizeName()
    {
        // Get random name in the selected faces and invert first letters
        if (_namesList.Count == _imageList.Count)
        {
            SName randName = new();
            string firstName = _namesList[FaceParts[UnityEngine.Random.Range(0, FaceParts.Count)].CurrentfaceID].FirstName;
            string lastName = _namesList[FaceParts[UnityEngine.Random.Range(0, FaceParts.Count)].CurrentfaceID].LastName;
            randName.FirstName = lastName[0].ToString().ToUpper() + firstName.Remove(0, 1);
            randName.LastName = firstName[0].ToString().ToUpper() + lastName.Remove(0, 1);

            NameText.text = randName.FirstName + " " + randName.LastName;
        }
    }

    private void SetLastPartPixelHeight()
    {
        // Bottom integrate all the remaining pixel height

        int accVal = 0;
        for (int i = 0; i < FaceParts.Count - 1; ++i)
        {
            accVal += FaceParts[i].PartPixelHeight;
        }

        FaceParts[^1].PartPixelHeight = _totalImageHeight - accVal;
    }
}
