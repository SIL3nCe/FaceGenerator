using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartHeightSlider : MonoBehaviour
{
    public TMP_Text PartNameText;
    public TMP_InputField PartInputField;
    public Slider PartSlider;

    private int _partID = 0;

    public void SetPartID(int partID)
    {
        _partID = partID;
        PartNameText.text = partID.ToString();
    }

    public void OnSliderValueChanged(float value)
    {
        PartInputField.text = ((int)value).ToString();
    }

    public void OnInputFieldValueChanged(string value)
    {
        int parseResult = 0;
        bool res = int.TryParse(value, out parseResult);
        if (res)
        {
            PartSlider.value = parseResult;
        }
        else
        {
            PartInputField.text = (PartSlider.value).ToString();
        }
    }
}
