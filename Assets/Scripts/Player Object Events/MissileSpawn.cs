using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSModularSystem;

public class MissileSpawn : MonoBehaviour
{
    public void OnMissileSpawn(PlayerObject commandCenter, GameActionData data)
    {
        if (RTSPlayer.Owns(commandCenter))
        {
            GameObject launchButton = (GameObject)Instantiate(NuclearLaunch.instance.launchButtonPrefab);
            launchButton.GetComponentInChildren<NuclearLaunchButton>().Init(GetComponent<PlayerObject>());

            NuclearLaunch.instance.SetButton(launchButton);
        }
    }
}
