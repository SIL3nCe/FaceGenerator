using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePartUI : MonoBehaviour
{
    public int FacePartID;

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
        FaceGenerator.Instance.LockPart(FacePartID);
    }
}
