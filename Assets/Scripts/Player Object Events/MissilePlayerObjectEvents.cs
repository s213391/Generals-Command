using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSModularSystem;

public class MissilePlayerObjectEvents : PlayerObjectEvents
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OnSpawn(bool localOwned)
    {
        if (localOwned)
        {
            GameObject launchButton = (GameObject)Instantiate(NuclearLaunch.instance.launchButtonPrefab);
            launchButton.GetComponentInChildren<NuclearLaunchButton>().Init(GetComponent<PlayerObject>());

            NuclearLaunch.instance.SetButton(launchButton);
        }
    }
}
