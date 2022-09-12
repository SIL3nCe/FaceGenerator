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
