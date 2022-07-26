using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using RTSModularSystem;
using RTSModularSystem.Selection;
using TMPro;

public class HUD : MonoBehaviour
{   
    public static HUD instance { get; private set; }
    
    [Header("Minimap")]
    public GameObject minimapExpanded;
    public GameObject minimapMinimised;
    [Header("Groups Menu")]
    public GameObject groupsMenuExpanded;
    public GameObject groupsMenuMinimised;
    public GameObject addToGroupMenu;
    [Header("Selection Box")]
    public GameObject selectionToggleExpanded;
    public GameObject selectionToggleMinimised;
    [Header("Selected Menu")]
    public GameObject selectedMenuExpanded;
    public GameObject selectedMenuMinimised;
    [Header("In-Game Menu")]
    public GameObject ingameMenuExpanded;
    public GameObject ingameMenuMinimised;
    public GameObject optionsMenuExpanded;
    public Slider panSpeedSlider;
    public Slider zoomSpeedSlider;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    private CameraController cameraController;
    private PlayerInput playerInput;

    //save button visual effects
    public Button saveButton;
    TextMeshProUGUI saveButtonText;


    //setup the singleton
    private void Start()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
        {
            instance = this;
            Settings.ApplySettings();
        }
    }


    //sets the HUD to default state on GUI initialise
    public void Init()
    {
        ToggleMinimap(false);
        ToggleGroupsMenu(false);
        ToggleSelection(false);
        ToggleInGameMenu(false);
        ToggleAddToGroupMenu(false);
        ToggleOptionsMenu(false);
    }


    //enables/disables the HUD
    public void SetEnabled(bool enable)
    {
        gameObject.SetActive(enable);
    }


    //opens/closes the minimap window while hiding/showing the button respectively
    public void ToggleMinimap(bool open)
    {
        minimapExpanded.SetActive(open);
        minimapMinimised.SetActive(!open);
    }


    //opens/closes the groups window while hiding/showing the button respectively
    public void ToggleGroupsMenu(bool open)
    {
        groupsMenuExpanded.SetActive(open);
        groupsMenuMinimised.SetActive(!open);
    }


    //switches single touch input between camera movement and dragging a selection box
    public void ToggleSelection(bool selectionOn)
    {
        selectionToggleExpanded.SetActive(selectionOn);
        selectionToggleMinimised.SetActive(!selectionOn);

        if (!playerInput)
            playerInput = PlayerInput.instance;
        if (!cameraController)
            cameraController = Camera.main.GetComponent<CameraController>();

        playerInput?.ToggleDragSelectionInputs(selectionOn);
        cameraController?.ToggleCameraInputs(!selectionOn);
    }


    //opens/closes the selected menu window while hiding/showing the button respectively
    public void ToggleSelectedMenu(bool open)
    {
        selectedMenuExpanded.SetActive(open);
        selectedMenuMinimised.SetActive(!open);

        //make sure the sub menu is closed when this menu is closed
        //if (!open)
            //ToggleAddToGroupMenu(false);
    }


    //opens the pause menu
    public void ToggleInGameMenu(bool open)
    {
        ingameMenuExpanded.SetActive(open);
        ingameMenuMinimised.SetActive(!open);
        optionsMenuExpanded.SetActive(false);
    }


    //opens/closes the add-to-group sub menu of the selection menu
    public void ToggleAddToGroupMenu(bool open)
    {
        addToGroupMenu.SetActive(open);
    }


    //finds and selects the closest engineer
    public void FindAvailableEngineer()
    { 
        List<PlayerObject> engineers = ObjectDataManager.GetPlayerObjectsOfType("Engineer", RTSPlayer.GetID());

        float closestDist = 9999.9f;
        PlayerObject closest = null;

        foreach (PlayerObject engineer in engineers)
        {
            float dist = (engineer.transform.position - CameraController.instance.target.transform.position).magnitude;
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = engineer;
            }
        }

        if (closest != null)
        {
            SelectionController.instance.DeselectAll();
            SelectionController.instance.Select(closest.GetComponent<RTSModularSystem.Selection.Selectable>());
        }
    }


    //deselects all objects and closes the actions and selected menus
    public void DeselectAll()
    {
        SelectionController.instance.DeselectAll();
    }


    //quits the game
    public void Quit()
    { 
        Application.Quit();
    }


    #region optionsMenu

    public void Load()
    {
        Settings.Load();
    }


    //opens the options sub menu
    public void ToggleOptionsMenu(bool open)
    {
        optionsMenuExpanded.SetActive(open);
        if (open)
        {
            Settings.Load();

            panSpeedSlider.value = Settings.panSpeed;
            zoomSpeedSlider.value = Settings.zoomSpeed;
            masterVolumeSlider.value = Settings.masterVolume;
            musicVolumeSlider.value = Settings.musicVolume;
            sfxVolumeSlider.value = Settings.sfxVolume;

            saveButton.interactable = false;
            if (!saveButtonText)
                saveButtonText = saveButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            saveButtonText.text = "Apply";
        }
    }


    //set values based on slider
    void Update()
    {
        if (optionsMenuExpanded.activeInHierarchy)
        {
            Settings.panSpeed = panSpeedSlider.value;
            Settings.zoomSpeed = zoomSpeedSlider.value;
            Settings.masterVolume = masterVolumeSlider.value;
            Settings.musicVolume = musicVolumeSlider.value;
            Settings.sfxVolume = sfxVolumeSlider.value;
        }
    }


    //enable save button
    public void EnableSave()
    {
        saveButton.interactable = true;
        if (!saveButtonText)
            saveButtonText = saveButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        saveButtonText.text = "Apply";
    }


    //save settings changes
    public void SaveChanges()
    {
        saveButton.interactable = false;
        if (!saveButtonText)
            saveButtonText = saveButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        saveButtonText.text = "Applied";

        Settings.Save();
    }


    //reset settings to default values
    public void ResetToDefaults()
    {
        Settings.ResetPrefs();

        panSpeedSlider.value = Settings.panSpeed;
        zoomSpeedSlider.value = Settings.zoomSpeed;
        masterVolumeSlider.value = Settings.masterVolume;
        musicVolumeSlider.value = Settings.musicVolume;
        sfxVolumeSlider.value = Settings.sfxVolume;

        saveButton.interactable = false;
        if (!saveButtonText)
            saveButtonText = saveButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        saveButtonText.text = "Applied";
    }

    #endregion

    #region groupsMenu


    public void AddToGroup(int groupNumber)
    {
        SelectionController.instance.AddSelectedToGroup(groupNumber);
    }


    public void EmptyGroup(int groupNumber)
    {
        SelectionController.instance.EmptyQuickSelectGroup(groupNumber);
    }


    public void SelectGroup(int groupNumber)
    {
        SelectionController.instance.SelectGroup(groupNumber);
    }    


    #endregion
}
