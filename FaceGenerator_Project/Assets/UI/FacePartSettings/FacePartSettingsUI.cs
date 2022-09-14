using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FacePartSettingsUI : MonoBehaviour
{
    public GameObject PartPanel;
    public GameObject PartHeightSliderPrefab;

    private List<GameObject> _partSliderList = new();

    public void OnAddPart()
    {
        GameObject slider = Instantiate(PartHeightSliderPrefab, PartPanel.transform);
        slider.GetComponent<PartHeightSlider>().SetPartID(_partSliderList.Count);

        _partSliderList.Add(slider);

        // Update panel height
        Rect panelRect = PartPanel.GetComponent<RectTransform>().rect;
        panelRect.height += 35.0f;
        PartPanel.GetComponent<RectTransform>().sizeDelta = panelRect.size;
    }

    public void OnRemoveLastPart()
    {
        if (_partSliderList.Count > 0)
        {
            Destroy(_partSliderList[^1]);
            _partSliderList.RemoveAt(_partSliderList.Count - 1);

            // Update panel height
            Rect panelRect = PartPanel.GetComponent<RectTransform>().rect;
            panelRect.height -= 35.0f;
            PartPanel.GetComponent<RectTransform>().sizeDelta = panelRect.size;
        }
    }

    public void OnSave()
    {

    }
}
