using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaceNameUI : MonoBehaviour
{
    public GameObject LockButton;

    public Sprite LockedIcon;
    public Sprite UnlockedIcon;

    public void OnLockClicked()
    {
        bool locked = FaceGenerator.Instance.LockName();
        LockButton.GetComponent<Image>().sprite = locked ? LockedIcon : UnlockedIcon;
    }

    public void OnRandomizeClicked()
    {
        FaceGenerator.Instance.RandomizeName();
    }
}
