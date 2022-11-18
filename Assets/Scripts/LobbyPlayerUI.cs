using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyPlayerUI : MonoBehaviour
{
    bool isOccupied;
    bool isLocalPlayer;
    LobbyPlayer player;

    [SerializeField]
    GameObject occupiedVisuals;
    [SerializeField]
    GameObject unoccupiedVisuals;
    [SerializeField]
    TMP_InputField nameField;
    [SerializeField]
    LockableDropdownList colourDropdown;
    [SerializeField]
    LockableDropdownList teamDropdown;
    [SerializeField]
    GameObject readyIcon;
    [SerializeField]
    GameObject notReadyIcon;


    //set up this slot and its dropdown lists
    public void Init()
    {
        isOccupied = false;
        nameField.onSelect.AddListener(NameFieldSelected);
        nameField.onEndEdit.AddListener(ServerLobby.instance.SetPlayerName);
        colourDropdown.Init(0);
        colourDropdown.onValueChanged.AddListener(ServerLobby.instance.SetChosenOption);
        teamDropdown.Init(1);
        teamDropdown.onValueChanged.AddListener(ServerLobby.instance.SetChosenOption);

        UpdateVisuals();
    }


    //called when a player is added or removed
    public void UpdateData(bool occupied, LobbyPlayer playerData)
    {
        isOccupied = occupied;

        if (isOccupied)
        {
            player = playerData;

            if (player.playerNumber == ServerLobby.instance.GetLocalPlayerData().playerNumber)
                isLocalPlayer = true;
            else
                isLocalPlayer = false;
        }

        UpdateVisuals();
    }


    //updates the details of the player in this slot
    public void UpdateVisuals()
    {
        occupiedVisuals.SetActive(isOccupied);
        unoccupiedVisuals.SetActive(!isOccupied);

        if (isOccupied)
        {
            if (isLocalPlayer)
                nameField.text = player.name + " (You)";
            else
                nameField.text = player.name;
            nameField.interactable = isLocalPlayer;

            readyIcon.SetActive(player.ready);
            notReadyIcon.SetActive(!player.ready);

            colourDropdown.SetSelectedOption(player.colourIndex);
            colourDropdown.SetInteractable(isLocalPlayer && !player.ready);
            colourDropdown.SetOptionsInteractable(ServerLobby.instance.GetAvailableColours());

            teamDropdown.SetSelectedOption(player.teamIndex);
            teamDropdown.SetInteractable(isLocalPlayer && ServerLobby.instance.TeamsEnabled() && !player.ready);
        }
    }


    //returns the given dropdown list
    public LockableDropdownList GetDropdown(int listID)
    {
        if (listID == 0)
            return colourDropdown;
        else
            return teamDropdown;
    }


    //removes the appended (You) from the text field when selected
    public void NameFieldSelected(string text)
    {
        if (text.Substring(text.Length - 6, 6) == " (You)")
            nameField.text = text.Substring(0, text.Length - 6);
    }
}
