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

    public float missileFlightTime = 5.0f;
    public float fadeOutTime = 1.0f;
    public float blackScreenDuration = 1.0f;
    public float fadeInTime = 1.0f;
    public float cameraDistance = 50.0f;

    public PlayerObjectData missileData;
    public GameObject launchButtonPrefab;
    GameObject launchButton;

    GameObject cameraTarget;
    bool missileLaunching = false;


    private void Start()
    {
        if (isLocalPlayer)
            instance = this;
    }


    //tells the server to notify clients that a nuclear missile is about to launch
    public void BeginLaunch(GameObject commandCenter, GameObject deadCenter)
    {
        CmdBeginLaunch(commandCenter, deadCenter);
    }


    [Command]
    //notifies clients that a missile is about to launch if it exists on the server
    void CmdBeginLaunch(GameObject commandCenter, GameObject deadCenter)
    { 
        missileLaunching = true;
        RpcShowLaunchTimer();
        NotificationManager.instance.nuclearCountdownTimer.GetComponentInChildren<GameTimer>().onTimerEnd.AddListener(delegate { MissileLaunch(commandCenter, deadCenter); });
    }


    [ClientRpc]
    //spawns and starts the launch counter on clients
    void RpcShowLaunchTimer()
    {
        NotificationManager.instance.nuclearCountdownTimer.SetActive(true);
    }


    [Server]
    public void MissileLaunch(GameObject commandCenter, GameObject deadCenter)
    {
        if (missileLaunching)
            RpcMissileLaunch(commandCenter, deadCenter);
    }


    [ClientRpc]
    void RpcMissileLaunch(GameObject commandCenter, GameObject deadCenter)
    {
        StartCoroutine(FadeToMissile(commandCenter.transform.GetChild(2), deadCenter.GetComponent<PlayerObject>()));
    }


    //disables all player interaction and fades to the missile
    public IEnumerator FadeToMissile(Transform missileTransform, PlayerObject deadCenter)
    {
        launchButton?.SetActive(false);
        ScreenFade.Out(fadeOutTime);
        yield return new WaitForSeconds(fadeOutTime);
        
        SelectionController.instance.DeselectAll();
        CameraController.instance.canMoveTarget = false;
        PlayerInput.instance.ToggleSingleSelectionInputs(false);
        PlayerInput.instance.ToggleDragSelectionInputs(false);
        PlayerInput.instance.ToggleMovementInputs(false);
        CombatManager.instance.enabled = false;
        HUD.instance.SetEnabled(false);

        cameraTarget = CameraController.instance.target;
        cameraTarget.transform.SetPositionAndRotation(missileTransform.position, missileTransform.rotation);
        if (RTSPlayer.Owns(deadCenter))
        {
            if (CameraController.instance.xAngle != 135.0f)
                CameraController.instance.xAngle = 135.0f;
            else
                CameraController.instance.xAngle = 45.0f;
        }
        CameraController.instance.currentDistance = cameraDistance;
        yield return new WaitForSeconds(blackScreenDuration);

        ScreenFade.In(fadeInTime);
        yield return new WaitForSeconds(fadeInTime);

        missileTransform.GetComponent<Animator>().speed = 1.0f;
        StartCoroutine(MissileFlight(missileTransform, deadCenter));
    }


    //sets the nuclear button variable
    public void SetButton(GameObject button)
    {
        launchButton = button;
    }    


    //move the camera target to follow the xz coordinates of the missile
    public IEnumerator MissileFlight(Transform missileTransform, PlayerObject deadCenter)
    {
        float timer = 0.0f;
        while (timer < missileFlightTime)
        { 
            timer += Time.deltaTime;
            
            Vector3 missilePosition = missileTransform.position;
            missilePosition.y = cameraTarget.transform.position.y;

            cameraTarget.transform.position = missilePosition;
            yield return null;
        }

        missileTransform.gameObject.SetActive(false);
        deadCenter.transform.GetChild(3).gameObject.SetActive(true);
        if (isServer)
            deadCenter.GetComponent<CommandCenterEvents>().OnDeath();
    }
}
