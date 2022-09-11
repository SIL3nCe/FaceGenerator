using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class FacePart : MonoBehaviour
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

    public void SetPositionBasedOnPixelHeightAccumulation(FacePart previousPart)
    {
        Vector2 partPos = PartRenderer.transform.position;

        // 100 pixel per unit by default, divide every thing by 100 to get y usable for transform
        if (previousPart == null)
        {
            partPos.y = PartPixelHeight * -0.005f;
        }
        else
        {
            partPos.y = previousPart.PartRenderer.transform.position.y - (previousPart.PartPixelHeight * 0.005f) - (PartPixelHeight * 0.005f);
        }

        PartRenderer.transform.position = partPos;
    }

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
