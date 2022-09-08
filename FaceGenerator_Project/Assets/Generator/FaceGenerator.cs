using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public struct SName
{
    public string FirstName;
    public string LastName;
}

public class FaceGenerator : MonoBehaviour
{
    public int BaseImageHeight = 512;

    //TODO dyn array of struct for this
    public SpriteRenderer FaceUpRenderer;
    public int FaceUpPixelHeight = 150;

    public SpriteRenderer FaceEyesRenderer;
    public int FaceEyesPixelHeight = 50;

    public SpriteRenderer FaceNoseRenderer;
    public int FaceNosePixelHeight = 150;

    public SpriteRenderer FaceBottomRenderer;
    public int FaceBottomPixelHeight = 162;

    private List<Texture2D> _imageList = new();

    private List<SName> _namesList = new();

    public TMP_Text NameText;

    void Start()
    {
        if (Directory.Exists("./Images/"))
        {
            DirectoryInfo dirInfo = new("./Images/");
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

        Texture2D tex = new Texture2D(0, 0);
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

        List<int> faceIds = new();

        int pixelHeightAccumulation = FaceUpPixelHeight;

        // Up
        int faceId = Random.Range(0, _imageList.Count);
        faceIds.Add(faceId);

        Texture2D baseTexture = _imageList[faceId];
        Texture2D texture_up = new(baseTexture.width, FaceUpPixelHeight);
        texture_up.SetPixels(baseTexture.GetPixels(0, baseTexture.height - pixelHeightAccumulation, baseTexture.width, FaceUpPixelHeight));
        texture_up.Apply();
        FaceUpRenderer.sprite = Sprite.Create(texture_up, new Rect(0.0f, 0.0f, texture_up.width, texture_up.height), new Vector2(0.5f, 0.5f));

        // Eyes
        faceId = Random.Range(0, _imageList.Count);
        faceIds.Add(faceId);

        pixelHeightAccumulation += FaceEyesPixelHeight;
        baseTexture = _imageList[faceId];
        Texture2D texture_eyes = new(baseTexture.width, FaceEyesPixelHeight);
        texture_eyes.SetPixels(baseTexture.GetPixels(0, baseTexture.height - pixelHeightAccumulation, baseTexture.width, FaceEyesPixelHeight));
        texture_eyes.Apply();
        FaceEyesRenderer.sprite = Sprite.Create(texture_eyes, new Rect(0.0f, 0.0f, texture_eyes.width, texture_eyes.height), new Vector2(0.5f, 0.5f));

        // Nose/Mouth
        faceId = Random.Range(0, _imageList.Count);
        faceIds.Add(faceId);

        pixelHeightAccumulation += FaceNosePixelHeight;
        baseTexture = _imageList[faceId];
        Texture2D texture_nose = new(baseTexture.width, FaceNosePixelHeight);
        texture_nose.SetPixels(baseTexture.GetPixels(0, baseTexture.height - pixelHeightAccumulation, baseTexture.width, FaceNosePixelHeight));
        texture_nose.Apply();
        FaceNoseRenderer.sprite = Sprite.Create(texture_nose, new Rect(0.0f, 0.0f, texture_nose.width, texture_nose.height), new Vector2(0.5f, 0.5f));

        // Bottom
        faceId = Random.Range(0, _imageList.Count);
        faceIds.Add(faceId);

        baseTexture = _imageList[faceId];
        Texture2D texture_bottom = new(baseTexture.width, FaceBottomPixelHeight);
        texture_bottom.SetPixels(baseTexture.GetPixels(0, 0, baseTexture.width, FaceBottomPixelHeight));
        texture_bottom.Apply();
        FaceBottomRenderer.sprite = Sprite.Create(texture_bottom, new Rect(0.0f, 0.0f, texture_bottom.width, texture_bottom.height), new Vector2(0.5f, 0.5f));

        // Get random name in the selected faces and invert first letters
        if (_namesList.Count == _imageList.Count)
        {
            SName randName = new();
            string firstName = _namesList[Random.Range(0, _namesList.Count)].FirstName;
            string lastName = _namesList[Random.Range(0, _namesList.Count)].LastName;
            randName.FirstName = lastName[0].ToString().ToUpper() + firstName.Remove(0, 1);
            randName.LastName = firstName[0].ToString().ToUpper() + lastName.Remove(0, 1);

            NameText.text = randName.FirstName+ " " + randName.LastName;
        }
    }

    private void OnValidate()
    {
        // Bottom integrate all the remaining pixel height
        FaceBottomPixelHeight = BaseImageHeight - (FaceNosePixelHeight + FaceEyesPixelHeight + FaceUpPixelHeight);
    }
}
