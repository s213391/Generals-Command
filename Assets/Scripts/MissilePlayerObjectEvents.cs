using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissilePlayerObjectEvents : PlayerObjectEvents
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OnSpawn()
    {
        GameObject launchButton = (GameObject)Instantiate(Resources.Load("Nuclear Button Canvas.obj"));

    }
}
