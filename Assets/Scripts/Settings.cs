using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings instance;

    public float brightness;
    public int shadowsPreset;
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
    public float panSpeed;
    public float zoomSpeed;
    

    //set up singleton
    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

        DontDestroyOnLoad(this.gameObject);
        CreateNewPrefs();
    }


    //create new player prefs on device
    void CreateNewPrefs()
    {
        if (!PlayerPrefs.HasKey("brightness"))
            PlayerPrefs.SetFloat("brightness", 0.5f);
        if (!PlayerPrefs.HasKey("shadowsPreset"))
            PlayerPrefs.SetInt("shadowsPreset", 1);
        if (!PlayerPrefs.HasKey("masterVolume"))
            PlayerPrefs.SetFloat("masterVolume", 0.5f); 
        if (!PlayerPrefs.HasKey("musicVolume"))
            PlayerPrefs.SetFloat("musicVolume", 0.5f); 
        if (!PlayerPrefs.HasKey("sfxVolume"))
            PlayerPrefs.SetFloat("sfxVolume", 0.5f); 
        if (!PlayerPrefs.HasKey("panSpeed"))
            PlayerPrefs.SetFloat("panSpeed", 5.0f); 
        if (!PlayerPrefs.HasKey("zoomSpeed"))
            PlayerPrefs.SetFloat("zoomSpeed", 2.0f);
    }


    //save the new settings values
    public void Save()
    {
        if (!(PlayerPrefs.GetFloat("brightness") == brightness))
            PlayerPrefs.SetFloat("brightness", 0.5f);
        if (!(PlayerPrefs.GetInt("shadowsPreset") == shadowsPreset))
            PlayerPrefs.SetInt("shadowsPreset", 1);
        if (!(PlayerPrefs.GetFloat("masterVolume") == masterVolume))
            PlayerPrefs.SetFloat("masterVolume", 0.5f);
        if (!(PlayerPrefs.GetFloat("musicVolume") == musicVolume))
            PlayerPrefs.SetFloat("musicVolume", 0.5f);
        if (!(PlayerPrefs.GetFloat("sfxVolume") == sfxVolume))
            PlayerPrefs.SetFloat("sfxVolume", 0.5f);
        if (!(PlayerPrefs.GetFloat("panSpeed") == panSpeed))
            PlayerPrefs.SetFloat("panSpeed", 5.0f);
        if (!(PlayerPrefs.GetFloat("zoomSpeed") == zoomSpeed))
            PlayerPrefs.SetFloat("zoomSpeed", 2.0f);

        PlayerPrefs.Save();
    }
}
