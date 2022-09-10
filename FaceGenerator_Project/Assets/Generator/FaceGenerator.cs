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

[Serializable]
public class SFacePart
{
    public string PartName;

    public SpriteRenderer PartRenderer;
    public int PartPixelHeight = 0;

    // Accumulation of pixel height of previous parts
    [NonSerialized]
    public int PixelHeightAccumulation = 0;

    // Id of the texture in _imageList used for this part
    [NonSerialized]
    public int CurrentfaceID = -1;

    [NonSerialized]
    public bool IsLocked = false;

    public void RandomizePart(List<Texture2D> imageList)
    {
        if (IsLocked)
            return;

        CurrentfaceID = UnityEngine.Random.Range(0, imageList.Count);

        ApplyCurrentFaceID(imageList);
    }

    public void LoadNextTextureID(List<Texture2D> imageList, bool next)
    {
        if (IsLocked)
            return;

        CurrentfaceID += imageList.Count + (next ? 1 : -1);
        CurrentfaceID %= imageList.Count;
        
        ApplyCurrentFaceID(imageList);
    }

    private void ApplyCurrentFaceID(List<Texture2D> imageList)
    {
        Texture2D baseTexture = imageList[CurrentfaceID];
        Texture2D partTexture = new(baseTexture.width, PartPixelHeight);
        partTexture.filterMode = FilterMode.Point;
        partTexture.SetPixels(baseTexture.GetPixels(0, baseTexture.height - PixelHeightAccumulation, baseTexture.width, PartPixelHeight));
        partTexture.Apply();

        PartRenderer.sprite = Sprite.Create(partTexture, new Rect(0.0f, 0.0f, partTexture.width, partTexture.height), new Vector2(0.5f, 0.5f));
    }
}

public class FaceGenerator : MonoBehaviour
{
    // TODO get base height based on smallest gathered image?
    public int BaseImageHeight = 512;

    // Face part data
    public List<SFacePart> FaceParts = new();

    // List of gathered images
    private readonly string _imageDirectoryPath = "./Images/";
    private List<Texture2D> _imageList = new();

    // Names gathered from images
    private List<SName> _namesList = new();

    public TMP_Text NameText;

    void Start()
    {
        int pixelHeightAccumulation = 0;
        foreach (SFacePart facePart in FaceParts)
        {
            pixelHeightAccumulation += facePart.PartPixelHeight;
            facePart.PixelHeightAccumulation = pixelHeightAccumulation;
        }

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

            GenerateFace();
        }
    }

    private void CreateTextureFromFilePath(string filePath)
    {
        byte[] bytes = System.IO.File.ReadAllBytes(filePath);

        Texture2D tex = new Texture2D(10, 10);
        tex.LoadImage(bytes);

        _imageList.Add(tex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GenerateFace();
        }
    }

    private void GenerateFace()
    {
        if (_imageList.Count == 0)
            return;

        // Get random image for every part and crop them
        foreach (SFacePart facePart in FaceParts)
        {
            facePart.RandomizePart(_imageList);
        }

        RandomizeName();
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

    private void OnValidate()
    {
        // Bottom integrate all the remaining pixel height

        int accVal = 0;
        for (int i = 0; i < FaceParts.Count - 1; ++i)
        {
            accVal += FaceParts[i].PartPixelHeight;
        }

        FaceParts[^1].PartPixelHeight = BaseImageHeight - accVal;
    }
}
