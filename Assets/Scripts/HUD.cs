using UnityEngine;
using RTSModularSystem;

public class HUD : MonoBehaviour
{
    public GameObject minimapExpanded;
    public GameObject minimapMinimised;
    public GameObject groupsMenuExpanded;
    public GameObject groupsMenuMinimised;
    public GameObject actionsMenuExpanded;
    public GameObject actionsMenuMinimised;
    public GameObject selectionToggleExpanded;
    public GameObject selectionToggleMinimised;
    public GameObject selectedMenuExpanded;
    public GameObject selectedMenuMinimised;
    public GameObject ingameMenuExpanded;
    public GameObject ingameMenuMinimised;
    public GameObject addToGroupMenuExpanded;
    public GameObject addToGroupMenuMinimised;

    private CameraController cameraController;
    private PlayerInput playerInput;
    

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


    //opens/closes the actions window while hiding/showing the button respectively
    public void ToggleActionsMenu(bool open)
    {
        actionsMenuExpanded.SetActive(open);
        actionsMenuMinimised.SetActive(!open);
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
    }


    //opens/closes the add-to-group sub menu of the selection menu
    public void ToggleAddToGroupMenu(bool open)
    {
        addToGroupMenuExpanded.SetActive(open);
        addToGroupMenuMinimised.SetActive(!open);
    }
}
