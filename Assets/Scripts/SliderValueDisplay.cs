using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderValueDisplay : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;
    public Slider slider;
    public int characters = 4;
    string value;

    //get the slider and value
    private void Start()
    {
        if (!slider)
            slider = transform.parent.GetComponent<Slider>();

        if (!textMeshProUGUI)
            textMeshProUGUI = GetComponent<TextMeshProUGUI>();

        SliderValueChange();
        slider.onValueChanged.AddListener(delegate { SliderValueChange(); });
    }


    //update text when slider value is changed
    public void SliderValueChange()
    {
        value = slider.value.ToString();
        if (value.Length > characters)
            value = value.Substring(0, characters);

        textMeshProUGUI.text = value;
    }
}
