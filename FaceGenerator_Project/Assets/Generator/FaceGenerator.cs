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
    public int PartPixelHeight;

    [NonSerialized]
    public int CurrentfaceID;

    [NonSerialized]
    public bool IsLocked;
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
        if (Directory.Exists(_imageDirectoryPath))
        {
            DirectoryInfo dirInfo = new(_imageDirectoryPath);
            foreach (FileInfo file in dirInfo.GetFiles())
            {
                CreateTextureFromFilePath(file.FullName);

                string[] nameSplit = file.Name.Split("_");
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

        int pixelHeightAccumulation = 0;

        // Get random image for every part and crop it
        foreach (SFacePart facePart in FaceParts)
        {
            pixelHeightAccumulation += facePart.PartPixelHeight;

            facePart.CurrentfaceID = UnityEngine.Random.Range(0, _imageList.Count);

            Texture2D baseTexture = _imageList[facePart.CurrentfaceID];
            Texture2D partTexture = new(baseTexture.width, facePart.PartPixelHeight);
            partTexture.SetPixels(baseTexture.GetPixels(0, baseTexture.height - pixelHeightAccumulation, baseTexture.width, facePart.PartPixelHeight));
            partTexture.Apply();

            facePart.PartRenderer.sprite = Sprite.Create(partTexture, new Rect(0.0f, 0.0f, partTexture.width, partTexture.height), new Vector2(0.5f, 0.5f));
        }

        // Get random name in the selected faces and invert first letters
        if (_namesList.Count == _imageList.Count)
        {
            SName randName = new();
            string firstName = _namesList[FaceParts[UnityEngine.Random.Range(0, FaceParts.Count)].CurrentfaceID].FirstName;
            string lastName = _namesList[FaceParts[UnityEngine.Random.Range(0, FaceParts.Count)].CurrentfaceID].LastName;
            randName.FirstName = lastName[0].ToString().ToUpper() + firstName.Remove(0, 1);
            randName.LastName = firstName[0].ToString().ToUpper() + lastName.Remove(0, 1);

            NameText.text = randName.FirstName+ " " + randName.LastName;
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
