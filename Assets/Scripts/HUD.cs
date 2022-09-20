using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using RTSModularSystem;
using DS_Selection;
using TMPro;

public class HUD : MonoBehaviour
{
    [Header("Minimap")]
    public GameObject minimapExpanded;
    public GameObject minimapMinimised;
    [Header("Groups Menu")]
    public GameObject groupsMenuExpanded;
    public GameObject groupsMenuMinimised;
    public GameObject addToGroupMenuExpanded;
    public GameObject addToGroupMenuMinimised;
    [Header("Selection Box")]
    public GameObject selectionToggleExpanded;
    public GameObject selectionToggleMinimised;
    public GameObject selectionCanvas;
    [Header("Selected Menu")]
    public GameObject selectedMenuExpanded;
    public GameObject selectedMenuMinimised;
    [Header("In-Game Menu")]
    public GameObject ingameMenuExpanded;
    public GameObject ingameMenuMinimised;
    public GameObject optionsMenuExpanded;
    public Slider panSpeedSlider;
    public Slider zoomSpeedSlider;

    private CameraController cameraController;
    private PlayerInput playerInput;

    //save button visual effects
    public Button saveButton;
    TextMeshProUGUI saveButtonText;


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
        selectionCanvas.SetActive(selectionOn);
        selectionToggleMinimised.SetActive(!selectionOn);

        if (!playerInput)
            playerInput = PlayerInput.instance;
        if (!cameraController)
            cameraController = Camera.main.GetComponent<CameraController>();

        playerInput.ToggleSelectionInputs(selectionOn);
        cameraController.ToggleCameraInputs(!selectionOn);
    }


    //opens/closes the selected menu window while hiding/showing the button respectively
    public void ToggleSelectedMenu(bool open)
    {
        selectedMenuExpanded.SetActive(open);
        selectedMenuMinimised.SetActive(!open);

        //make sure the sub menu is closed when this menu is closed
        if (!open)
            ToggleAddToGroupMenu(false);
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
        addToGroupMenuExpanded.SetActive(open);
        addToGroupMenuMinimised.SetActive(!open);
    }


    //finds and selects the closest engineer
    public void FindAvailableEngineer()
    { 
        List<PlayerObject> engineers = ObjectDataManager.GetPlayerObjectsOfType("Engineer", RTSPlayer.GetID());
        Vector3 camPos = CameraController.instance.target.transform.position;

        float closestDist = 9999.9f;
        PlayerObject closest = null;

        foreach (PlayerObject engineer in engineers)
        {
            float dist = (engineer.transform.position - camPos).magnitude;
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = engineer;
            }
        }

        if (closest != null)
        {
            SelectionController.instance.DeselectAll();
            SelectionController.instance.Select(closest.GetComponent<DS_Selection.Selectable>());
        }
    }


    //quits the game
    public void Quit()
    { 
        Application.Quit();
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

            saveButton.interactable = false;
            if (!saveButtonText)
                saveButtonText = saveButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            saveButtonText.text = "Apply Settings";
        }
    }


    //set values based on slider
    void Update()
    {
        if (optionsMenuExpanded.activeInHierarchy)
        {
            Settings.panSpeed = panSpeedSlider.value;
            Settings.zoomSpeed = zoomSpeedSlider.value;
        }
    }


    //enable save button
    public void EnableSave()
    {
        saveButton.interactable = true;
        if (!saveButtonText)
            saveButtonText = saveButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        saveButtonText.text = "Apply Settings";
    }


    //save settings changes
    public void SaveChanges()
    {
        saveButton.interactable = false;
        if (!saveButtonText)
            saveButtonText = saveButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        saveButtonText.text = "Settings Applied";

        Settings.Save();
    }
}
