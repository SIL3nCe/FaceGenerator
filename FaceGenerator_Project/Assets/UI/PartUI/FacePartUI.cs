using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FacePartUI : MonoBehaviour
{
    public int FacePartID;

    public GameObject LockButton;

    public Sprite LockedIcon;
    public Sprite UnlockedIcon;
    private void Start()
    {
        // Scale world space UI based on tota generated pixel height which is changing camera size. Initially configured for 512x512
        Vector3 scale = transform.localScale;
        scale *= FaceGenerator.Instance.TotalImageHeight / 512.0f;
        transform.localScale = scale;
    }

    public void OnPreviousClicked()
    {
        FaceGenerator.Instance.LoadPreviousPart(FacePartID);
    }

    public void OnNextClicked()
    {
        FaceGenerator.Instance.LoadNextPart(FacePartID);
    }

    public void OnLockClicked()
    {
        bool locked = FaceGenerator.Instance.LockPart(FacePartID);
        LockButton.GetComponent<Image>().sprite = locked ? LockedIcon : UnlockedIcon;
    }

    public void OnRandomizeClicked()
    {
        FaceGenerator.Instance.RandomizePart(FacePartID);
    }
}
