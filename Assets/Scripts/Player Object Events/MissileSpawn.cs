using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSModularSystem;
using Mirror;

public class MissileSpawn : NetworkBehaviour
{
    public GameObject missile;
    public Animator hatchAnimator;
    public Animator missileAnimator;

    void Start()
    {
        hatchAnimator.speed = 0.0f;
        missileAnimator.speed = 0.0f;
    }


    public void OnMissileSpawn(PlayerObject commandCenter, GameActionData data)
    {
        if (RTSPlayer.Owns(commandCenter))
        {
            GameObject launchButton = (GameObject)Instantiate(NuclearLaunch.instance.launchButtonPrefab);
            launchButton.GetComponentInChildren<NuclearLaunchButton>().Init(gameObject, RTSPlayer.otherPlayer.gameObject);

            NuclearLaunch.instance.SetButton(launchButton);

            CmdActivateMissile();
        }
    }


    [Command]
    public void CmdActivateMissile()
    {
        RpcActivateMissile();
    }


    [ClientRpc]
    public void RpcActivateMissile()
    {
        missile.SetActive(true);
        hatchAnimator.speed = 2.0f;
    }



}
