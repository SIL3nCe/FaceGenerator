using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceNameUI : MonoBehaviour
{
    public void OnLockClicked()
    {
        FaceGenerator.Instance.LockName();
    }

    public void OnRandomizeClicked()
    {
        FaceGenerator.Instance.RandomizeName();
    }
}
