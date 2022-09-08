using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FaceGenerator : MonoBehaviour
{
    public int BaseImageHeight = 512;

    public SpriteRenderer FaceUpRenderer;
    public int FaceUpPixelHeight = 150;

    public SpriteRenderer FaceEyesRenderer;
    public int FaceEyesPixelHeight = 50;

    public SpriteRenderer FaceNoseRenderer;
    public int FaceNosePixelHeight = 150;

    public SpriteRenderer FaceBottomRenderer;
    public int FaceBottomPixelHeight = 162;

    private List<Texture2D> _imageList = new();

    void Start()
    {
        if (Directory.Exists("./Images/"))
        {
            DirectoryInfo dirInfo = new("./Images/");
            foreach (FileInfo file in dirInfo.GetFiles())
            {
                CreateTextureFromFilePath(file.FullName);
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

        int pixelHeightAccumulation = FaceUpPixelHeight;

        // Up
        Texture2D baseTexture = _imageList[Random.Range(0, _imageList.Count)];
        Texture2D texture_up = new(baseTexture.width, FaceUpPixelHeight);
        texture_up.SetPixels(baseTexture.GetPixels(0, baseTexture.height - pixelHeightAccumulation, baseTexture.width, FaceUpPixelHeight));
        texture_up.Apply();
        FaceUpRenderer.sprite = Sprite.Create(texture_up, new Rect(0.0f, 0.0f, texture_up.width, texture_up.height), new Vector2(0.5f, 0.5f));

        // Eyes
        pixelHeightAccumulation += FaceEyesPixelHeight;
        baseTexture = _imageList[Random.Range(0, _imageList.Count)];
        Texture2D texture_eyes = new(baseTexture.width, FaceEyesPixelHeight);
        texture_eyes.SetPixels(baseTexture.GetPixels(0, baseTexture.height - pixelHeightAccumulation, baseTexture.width, FaceEyesPixelHeight));
        texture_eyes.Apply();
        FaceEyesRenderer.sprite = Sprite.Create(texture_eyes, new Rect(0.0f, 0.0f, texture_eyes.width, texture_eyes.height), new Vector2(0.5f, 0.5f));

        // Nose/Mouth
        pixelHeightAccumulation += FaceNosePixelHeight;
        baseTexture = _imageList[Random.Range(0, _imageList.Count)];
        Texture2D texture_nose = new(baseTexture.width, FaceNosePixelHeight);
        texture_nose.SetPixels(baseTexture.GetPixels(0, baseTexture.height - pixelHeightAccumulation, baseTexture.width, FaceNosePixelHeight));
        texture_nose.Apply();
        FaceNoseRenderer.sprite = Sprite.Create(texture_nose, new Rect(0.0f, 0.0f, texture_nose.width, texture_nose.height), new Vector2(0.5f, 0.5f));

        // Bottom
        baseTexture = _imageList[Random.Range(0, _imageList.Count)];
        Texture2D texture_bottom = new(baseTexture.width, FaceBottomPixelHeight);
        texture_bottom.SetPixels(baseTexture.GetPixels(0, 0, baseTexture.width, FaceBottomPixelHeight));
        texture_bottom.Apply();
        FaceBottomRenderer.sprite = Sprite.Create(texture_bottom, new Rect(0.0f, 0.0f, texture_bottom.width, texture_bottom.height), new Vector2(0.5f, 0.5f));
    }

    private void OnValidate()
    {
        // Bottom integrate all the remaining pixel height
        FaceBottomPixelHeight = BaseImageHeight - (FaceNosePixelHeight + FaceEyesPixelHeight + FaceUpPixelHeight);
    }
}
