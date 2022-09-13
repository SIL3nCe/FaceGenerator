using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class FacePart : MonoBehaviour
{
    public int FacePartID = 0;

    // Pixel height
    [NonSerialized]
    public int PartPixelHeightPercentage = 0;
    [NonSerialized]
    public int PartPixelHeight;
    // Accumulation of pixel height of previous parts
    [NonSerialized]
    public int PixelHeightAccumulation = 0;

    // Id of the texture in _imageList used for this part
    [NonSerialized]
    public int CurrentfaceID = -1;
    private Texture2D _partTexture;

    // Lock status
    [NonSerialized]
    public bool IsLocked = false;

    // Part UI
    public GameObject FacePartUIPrefab;
    private GameObject _facePartUI;

    private void Awake()
    {
        _partTexture = new(0, 0);
    }

    public void Initialize(int partID, int heightPercentage)
    {
        FacePartID = partID;
        PartPixelHeightPercentage = heightPercentage;

        _facePartUI = Instantiate(FacePartUIPrefab);
        _facePartUI.GetComponent<FacePartUI>().FacePartID = FacePartID;
    }

    public void SetPositionBasedOnPixelHeightAccumulation(FacePart previousPart)
    {
        Vector2 partPos = transform.position;

        // 100 pixel per unit by default, divide every thing by 100 to get y usable for transform
        if (previousPart == null)
        {
            partPos.y = PartPixelHeight * -0.005f;
        }
        else
        {
            partPos.y = previousPart.transform.position.y - (previousPart.PartPixelHeight * 0.005f) - (PartPixelHeight * 0.005f);
        }

        transform.position = partPos;

        partPos.x = FaceGenerator.Instance.TotalImageHeight * 0.005f;
        partPos.y += partPos.x;
        _facePartUI.transform.localPosition = partPos;
    }

    public void RandomizePart(List<Texture2D> imageList)
    {
        if (IsLocked)
            return;

        CurrentfaceID = UnityEngine.Random.Range(0, imageList.Count);

        ApplyCurrentFaceID(imageList);
    }

    public void ReversePart()
    {
        if (IsLocked || PartPixelHeight == 0)
            return;

        Color[] pixels = _partTexture.GetPixels();

        for (int i = 0; i < PartPixelHeight; ++i)
        {
            System.Array.Reverse(pixels, i * FaceGenerator.Instance.TotalImageHeight, FaceGenerator.Instance.TotalImageHeight);
        }

        _partTexture.SetPixels(pixels);
        _partTexture.Apply();
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
        if (PartPixelHeight == 0)
            return;

        Texture2D baseTexture = imageList[CurrentfaceID];

        _partTexture.Reinitialize(FaceGenerator.Instance.TotalImageHeight, PartPixelHeight);
        _partTexture.filterMode = FilterMode.Point;
        Color[] pixels = baseTexture.GetPixels(0, baseTexture.height - PixelHeightAccumulation, FaceGenerator.Instance.TotalImageHeight, PartPixelHeight);
        _partTexture.SetPixels(pixels);
        _partTexture.Apply();

        GetComponent<SpriteRenderer>().sprite = Sprite.Create(_partTexture, new Rect(0.0f, 0.0f, _partTexture.width, _partTexture.height), new Vector2(0.5f, 0.5f));

        if (UnityEngine.Random.Range(0, 2) == 1)
            ReversePart();
    }
}
