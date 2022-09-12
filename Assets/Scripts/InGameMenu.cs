using UnityEngine;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    //all 8 sliders
    public Slider brightnessSlider;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider panSpeedSlider;
    public Slider zoomSpeedSlider;
    public Slider zoomMinSlider;
    public Slider zoomMaxSlider;
    
    
    //load settings and set slider values
    public void Open()
    {
        Settings.Load();

        brightnessSlider.value = Settings.brightness;
        masterVolumeSlider.value = Settings.masterVolume;
        musicVolumeSlider.value = Settings.musicVolume;
        sfxVolumeSlider.value = Settings.sfxVolume;
        panSpeedSlider.value = Settings.panSpeed;
        zoomSpeedSlider.value = Settings.zoomSpeed;
        zoomMinSlider.value = Settings.zoomMin;
        zoomMaxSlider.value = Settings.zoomMax;
    }


    //set values based on slider
    void Update()
    {
         Settings.brightness = brightnessSlider.value;
         Settings.masterVolume = masterVolumeSlider.value;
         Settings.musicVolume = musicVolumeSlider.value;
         Settings.sfxVolume = sfxVolumeSlider.value;
         Settings.panSpeed = panSpeedSlider.value;
         Settings.zoomSpeed = zoomSpeedSlider.value;
         Settings.zoomMin = zoomMinSlider.value;
         Settings.zoomMax = zoomMaxSlider.value;
    }
}
