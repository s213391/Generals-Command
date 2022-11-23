using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSModularSystem;

public class TellCommandCenterNukeBuilt : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnMissileSpawn(PlayerObject po, GameActionData data)
    {
        po.GetComponent<MissileSpawn>().OnMissileSpawn(po, data);
    }
}
