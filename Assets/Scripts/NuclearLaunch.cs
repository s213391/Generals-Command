using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using RTSModularSystem;
using RTSModularSystem.Selection;
using RTSModularSystem.BasicCombat;

public class NuclearLaunch : NetworkBehaviour
{
    public static NuclearLaunch instance { get; private set; }

    public float fadeOutTime = 1.0f;
    public float blackScreenDuration = 1.0f;
    public float fadeInTime = 1.0f;
    public float cameraDistance = 50.0f;

    public PlayerObjectData missileData;
    public GameObject launchButtonPrefab;
    public GameObject launchCountdownPrefab;

    GameObject cameraTarget;
    bool missileLaunching = false;


    private void Start()
    {
        if (isLocalPlayer)
            instance = this;
    }


    [Server]
    //tells the server to notify clients that a nuclear missile is about to launch
    public void BeginLaunch(PlayerObject missile)
    {
        CmdBeginLaunch(missile.gameObject);
    }


    [Command]
    //notifies clients that a missile is about to launch if it exists on the server
    void CmdBeginLaunch(GameObject missileObject)
    { 
        PlayerObject missile = missileObject?.GetComponent<PlayerObject>();
        if (!missile || missile.data != missileData)
            return;

        missileLaunching = true;
        RpcShowLaunchTimer(missileObject);
    }


    [ClientRpc]
    //spawns and starts the launch counter on clients
    void RpcShowLaunchTimer(GameObject missileObject)
    {
        Transform missileTransform = missileObject.transform;
        GameObject launchButton = Instantiate(launchButtonPrefab);
        launchButton.GetComponent<GameTimer>().onTimerEnd.AddListener(delegate { StartCoroutine(FadeToMissile(missileTransform)); });
    }


    //disables all player interaction and fades to the missile
    public IEnumerator FadeToMissile(Transform missileTransform)
    {
        if (!missileLaunching)
            yield break;
        
        ScreenFade.Out(fadeOutTime);
        yield return new WaitForSeconds(fadeOutTime);
        
        SelectionController.instance.DeselectAll();
        PlayerInput.instance.ToggleSingleSelectionInputs(false);
        PlayerInput.instance.ToggleDragSelectionInputs(false);
        PlayerInput.instance.ToggleMovementInputs(false);
        CombatManager.instance.enabled = false;
        HUD.instance.SetEnabled(false);

        cameraTarget = CameraController.instance.target;
        cameraTarget.transform.SetPositionAndRotation(missileTransform.position, missileTransform.rotation);
        CameraController.instance.currentDistance = cameraDistance;
        yield return new WaitForSeconds(blackScreenDuration);

        ScreenFade.In(fadeInTime);
        yield return new WaitForSeconds(fadeInTime);

        StartCoroutine(FollowMissile(missileTransform));
    }


    //move the camera target to follow the xz coordinates of the missile
    public IEnumerator FollowMissile(Transform missileTransform)
    {
        while (true)
        { 
            Vector3 missilePosition = missileTransform.position;
            missilePosition.y = cameraTarget.transform.position.y;

            cameraTarget.transform.position = missilePosition;
        }
    }
}
