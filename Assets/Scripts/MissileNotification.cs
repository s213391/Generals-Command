using UnityEngine;
using RTSModularSystem;

public class MissileNotification : MonoBehaviour
{
    public void MissileBuildStart()
    {
        RTSPlayer.localPlayer.CmdMissileBuildStart();
    }
}
