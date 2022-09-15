using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaceNameUI : MonoBehaviour
{
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
