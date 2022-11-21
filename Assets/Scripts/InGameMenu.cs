using UnityEngine;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    //all sliders
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider panSpeedSlider;
    public Slider zoomSpeedSlider;
    
    
    //load settings and set slider values
    public void Open()
    {
        Settings.Load();

        masterVolumeSlider.value = Settings.masterVolume;
        musicVolumeSlider.value = Settings.musicVolume;
        sfxVolumeSlider.value = Settings.sfxVolume;
        panSpeedSlider.value = Settings.panSpeed;
        zoomSpeedSlider.value = Settings.zoomSpeed;
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
}
