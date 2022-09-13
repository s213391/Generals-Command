using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    //all 7 sliders
    public Slider brightnessSlider;
    public Slider shadowsSlider;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider panSpeedSlider;
    public Slider zoomSpeedSlider;
    
    
    //load settings and set slider values
    public void Open()
    {
        Settings.Load();

        brightnessSlider.value = Settings.brightness;
        shadowsSlider.value = Settings.shadowsPreset;
        masterVolumeSlider.value = Settings.masterVolume;
        musicVolumeSlider.value = Settings.musicVolume;
        sfxVolumeSlider.value = Settings.sfxVolume;
        panSpeedSlider.value = Settings.panSpeed;
        zoomSpeedSlider.value = Settings.zoomSpeed;
    }


    //set values based on slider
    void Update()
    {
         Settings.brightness = brightnessSlider.value;
         //Settings.shadowsPreset = shadowsSlider.value;
         Settings.masterVolume = masterVolumeSlider.value;
         Settings.musicVolume = musicVolumeSlider.value;
         Settings.sfxVolume = sfxVolumeSlider.value;
         Settings.panSpeed = panSpeedSlider.value;
         Settings.zoomSpeed = zoomSpeedSlider.value;
    }
}
