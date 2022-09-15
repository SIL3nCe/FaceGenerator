using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FacePartSettingsUI : MonoBehaviour
{
    public GameObject PartPanel;
    public GameObject PartHeightSliderPrefab;

    private List<GameObject> _partSliderList = new();

    private static FacePartSettingsUI _instance = null;
    public static FacePartSettingsUI Instance => _instance;

    private void Awake()
    {
        _instance = this;
    }

    public void CreatePartSlider(int defaultValue)
    {
        GameObject slider = Instantiate(PartHeightSliderPrefab, PartPanel.transform);
        slider.GetComponent<PartHeightSlider>().Initialize(_partSliderList.Count, defaultValue);

        _partSliderList.Add(slider);

        // Update panel height
        Rect panelRect = PartPanel.GetComponent<RectTransform>().rect;
        panelRect.height += 40.0f;
        PartPanel.GetComponent<RectTransform>().sizeDelta = panelRect.size;
    }

    public void SetPartSliderValue(int partID, int newValue)
    {
        _partSliderList[partID].GetComponent<PartHeightSlider>().ForceSliderValue(newValue);
    }

    public void OnAddPartClicked()
    {
        CreatePartSlider(20);
    }

    public void OnRemoveLastPartClicked()
    {
        if (_partSliderList.Count > 0)
        {
            Destroy(_partSliderList[^1]);
            _partSliderList.RemoveAt(_partSliderList.Count - 1);

            // Update panel height
            Rect panelRect = PartPanel.GetComponent<RectTransform>().rect;
            panelRect.height -= 40.0f;
            PartPanel.GetComponent<RectTransform>().sizeDelta = panelRect.size;
        }
    }

    public void OnSaveButtonClicked()
    {
        FaceGenerator.Instance.SaveCurrentPartHeights();
    }

    public void OnDisplayButtonClicked()
    {
        PartPanel.SetActive(!PartPanel.activeSelf);
    }
}
