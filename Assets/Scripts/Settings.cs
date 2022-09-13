using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings instance;

    public static float brightness;
    public static int shadowsPreset;
    public static float masterVolume;
    public static float musicVolume;
    public static float sfxVolume;
    public static float panSpeed;
    public static float zoomSpeed;
    public static float zoomMin;
    public static float zoomMax;
    

    //set up singleton
    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

        DontDestroyOnLoad(this.gameObject);
        CreateNewPrefs();
        Load();
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
        if (!PlayerPrefs.HasKey("zoomMin"))
            PlayerPrefs.SetFloat("zoomMin", 10.0f);
        if (!PlayerPrefs.HasKey("zoomMax"))
            PlayerPrefs.SetFloat("zoomMax", 70.0f);

        Load();
        PlayerPrefs.Save();
    }


    //read the settings values from the files
    public static void Load()
    {
        brightness = PlayerPrefs.GetFloat("brightness");
        shadowsPreset = PlayerPrefs.GetInt("shadowsPreset");
        masterVolume = PlayerPrefs.GetFloat("masterVolume");
        musicVolume = PlayerPrefs.GetFloat("musicVolume");
        sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
        panSpeed = PlayerPrefs.GetFloat("panSpeed");
        zoomSpeed = PlayerPrefs.GetFloat("zoomSpeed");
        zoomMin = PlayerPrefs.GetFloat("zoomMin");
        zoomMax = PlayerPrefs.GetFloat("zoomMax");
    }


    //save the new settings values
    public static void Save()
    {
        if (!(PlayerPrefs.GetFloat("brightness") == brightness))
            PlayerPrefs.SetFloat("brightness", brightness);
        if (!(PlayerPrefs.GetInt("shadowsPreset") == shadowsPreset))
            PlayerPrefs.SetInt("shadowsPreset", shadowsPreset);
        if (!(PlayerPrefs.GetFloat("masterVolume") == masterVolume))
            PlayerPrefs.SetFloat("masterVolume", masterVolume);
        if (!(PlayerPrefs.GetFloat("musicVolume") == musicVolume))
            PlayerPrefs.SetFloat("musicVolume", musicVolume);
        if (!(PlayerPrefs.GetFloat("sfxVolume") == sfxVolume))
            PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        if (!(PlayerPrefs.GetFloat("panSpeed") == panSpeed))
            PlayerPrefs.SetFloat("panSpeed", panSpeed);
        if (!(PlayerPrefs.GetFloat("zoomSpeed") == zoomSpeed))
            PlayerPrefs.SetFloat("zoomSpeed", zoomSpeed);
        if (!(PlayerPrefs.GetFloat("zoomMin") == zoomMin))
            PlayerPrefs.SetFloat("zoomMin", zoomMin);
        if (!(PlayerPrefs.GetFloat("zoomMax") == zoomMax))
            PlayerPrefs.SetFloat("zoomMax", zoomMax);

        ApplySettings();
        PlayerPrefs.Save();
    }


    //update the scripts that use these settings
    public static void ApplySettings()
    {
        if (RTSModularSystem.CameraController.instance != null)
        {
            RTSModularSystem.CameraController.instance.movementSpeed = panSpeed;
            RTSModularSystem.CameraController.instance.zoomSpeed = zoomSpeed;
            RTSModularSystem.CameraController.instance.minZoom = zoomMin;
            RTSModularSystem.CameraController.instance.maxZoom = zoomMax;
        }
    }
}
