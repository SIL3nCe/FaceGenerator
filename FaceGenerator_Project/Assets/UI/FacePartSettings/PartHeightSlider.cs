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

    public void Initialize(int partID, int defaultValue)
    {
        _partID = partID;
        PartNameText.text = partID.ToString();
        
        ForceSliderValue(defaultValue);
    }

    public void ForceSliderValue(int newValue)
    {

        PartSlider.SetValueWithoutNotify(newValue);
        PartInputField.text = newValue.ToString();
    }

    public void OnSliderValueChanged(float value)
    {
        PartInputField.text = ((int)value).ToString();
        FaceGenerator.Instance.OnPartSliderUpdated(_partID, (int)value);
    }

    public void OnInputFieldValueChanged(string value)
    {
        int parseResult = 0;
        bool res = int.TryParse(value, out parseResult);
        if (res)
        {
            parseResult = Mathf.Clamp(parseResult, 0, 100);
            PartSlider.value = parseResult;
            FaceGenerator.Instance.OnPartSliderUpdated(_partID, parseResult);
        }
        else
        {
            PartInputField.text = (PartSlider.value).ToString();
        }
    }
}
