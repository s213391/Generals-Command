using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTSModularSystem;

public class BuildMode : MonoBehaviour
{
    public Button confirmButton;
    public Button cancelButton;

    
    public void StartBuildMode(PlayerObject po, GameActionData data)
    {
        confirmButton.onClick.AddListener(delegate { RTSPlayer.localPlayer.SetTrigger(po, data); });
        PlayerInput.instance.ToggleMovementInputs(false);
    }


    public void EndBuildMode()
    {
        confirmButton.onClick.RemoveAllListeners();
        PlayerInput.instance.ToggleMovementInputs(true);
    }
}
