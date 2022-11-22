using UnityEngine;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    public static Settings instance;

    public AudioMixer audioMixer;

    public static float masterVolume;
    public static float musicVolume;
    public static float sfxVolume;
    public static float panSpeed;
    public static float zoomSpeed;
    

    //set up singleton
    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

        DontDestroyOnLoad(this.gameObject);
        CreateNewPrefs();
        ApplySettings();
    }


    //create new player prefs on device
    void CreateNewPrefs()
    {
        if (!PlayerPrefs.HasKey("masterVolume"))
            PlayerPrefs.SetFloat("masterVolume", -16.0f); 
        if (!PlayerPrefs.HasKey("musicVolume"))
            PlayerPrefs.SetFloat("musicVolume", 0.0f); 
        if (!PlayerPrefs.HasKey("sfxVolume"))
            PlayerPrefs.SetFloat("sfxVolume", 0.0f); 
        if (!PlayerPrefs.HasKey("panSpeed"))
            PlayerPrefs.SetFloat("panSpeed", 5.0f); 
        if (!PlayerPrefs.HasKey("zoomSpeed"))
            PlayerPrefs.SetFloat("zoomSpeed", 2.0f);

        Load();
        PlayerPrefs.Save();
    }


    //read the settings values from the files
    public static void Load()
    {
        masterVolume = PlayerPrefs.GetFloat("masterVolume");
        musicVolume = PlayerPrefs.GetFloat("musicVolume");
        sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
        panSpeed = PlayerPrefs.GetFloat("panSpeed");
        zoomSpeed = PlayerPrefs.GetFloat("zoomSpeed");
    }


    //save the new settings values
    public static void Save()
    {
        if (PlayerPrefs.GetFloat("masterVolume") != masterVolume)
            PlayerPrefs.SetFloat("masterVolume", masterVolume);
        if (PlayerPrefs.GetFloat("musicVolume") != musicVolume)
            PlayerPrefs.SetFloat("musicVolume", musicVolume);
        if (PlayerPrefs.GetFloat("sfxVolume") != sfxVolume)
            PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        if (PlayerPrefs.GetFloat("panSpeed") != panSpeed)
            PlayerPrefs.SetFloat("panSpeed", panSpeed);
        if (PlayerPrefs.GetFloat("zoomSpeed") != zoomSpeed)
            PlayerPrefs.SetFloat("zoomSpeed", zoomSpeed);

        ApplySettings();
        PlayerPrefs.Save();
    }


    //reset player prefs to default values on device
    public static void ResetPrefs()
    {
        masterVolume = -16.0f;
        musicVolume = -20.0f;
        sfxVolume = 0.0f;
        panSpeed = 5.0f;
        zoomSpeed = 2.0f;

        Save();
    }


    private void Update()
    {
        instance.audioMixer.SetFloat("masterVol", masterVolume);
        instance.audioMixer.SetFloat("sfxVol", sfxVolume);
        instance.audioMixer.SetFloat("musicVol", musicVolume);
    }


    //update the scripts that use these settings
    public static void ApplySettings()
    {
        if (RTSModularSystem.CameraController.instance != null)
        {
            RTSModularSystem.CameraController.instance.movementSpeed = panSpeed;
            RTSModularSystem.CameraController.instance.zoomSpeed = zoomSpeed;
        }

        instance.audioMixer.SetFloat("masterVol", masterVolume);
        instance.audioMixer.SetFloat("sfxVol", sfxVolume);
        instance.audioMixer.SetFloat("musicVol", musicVolume);
    }
}
