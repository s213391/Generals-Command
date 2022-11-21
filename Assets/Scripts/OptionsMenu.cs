using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    //all sliders
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider panSpeedSlider;
    public Slider zoomSpeedSlider;

    //save button
    public Button saveButton;
    TextMeshProUGUI textMeshProUGUI;


    //load settings and set slider values
    public void Open()
    {
        Settings.Load();

        masterVolumeSlider.value = Settings.masterVolume;
        musicVolumeSlider.value = Settings.musicVolume;
        sfxVolumeSlider.value = Settings.sfxVolume;
        panSpeedSlider.value = Settings.panSpeed;
        zoomSpeedSlider.value = Settings.zoomSpeed;

        saveButton.interactable = false;
        if (!textMeshProUGUI)
            textMeshProUGUI = saveButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textMeshProUGUI.text = "Save";
    }


    //set values based on slider
    void Update()
    {
        Settings.masterVolume = masterVolumeSlider.value;
        Settings.musicVolume = musicVolumeSlider.value;
        Settings.sfxVolume = sfxVolumeSlider.value;
        Settings.panSpeed = panSpeedSlider.value;
        Settings.zoomSpeed = zoomSpeedSlider.value;
    }


    //enable save button
    public void EnableSave()
    {
        saveButton.interactable = true;
        if (!textMeshProUGUI)
            textMeshProUGUI = saveButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textMeshProUGUI.text = "Save";
    }


    //save options
    public void SaveChanges()
    {
        saveButton.interactable = false;
        if (!textMeshProUGUI)
            textMeshProUGUI = saveButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textMeshProUGUI.text = "Saved";

        Settings.Save();
    }


    //resets settings to default values
    public void ResetToDefault()
    {
        Settings.ResetPrefs();

        masterVolumeSlider.value = Settings.masterVolume;
        musicVolumeSlider.value = Settings.musicVolume;
        sfxVolumeSlider.value = Settings.sfxVolume;
        panSpeedSlider.value = Settings.panSpeed;
        zoomSpeedSlider.value = Settings.zoomSpeed;

        saveButton.interactable = false;
        if (!textMeshProUGUI)
            textMeshProUGUI = saveButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textMeshProUGUI.text = "Saved";
    }
}
