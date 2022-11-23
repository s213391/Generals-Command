using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTSModularSystem;

public class NuclearLaunchButton : MonoBehaviour
{
    // Start is called before the first frame update
    public void Init(GameObject commandCenter, GameObject deadCenter)
    {
        GetComponent<Button>().onClick.AddListener(delegate { NuclearLaunch.instance.BeginLaunch(commandCenter, deadCenter); });
    }
}
