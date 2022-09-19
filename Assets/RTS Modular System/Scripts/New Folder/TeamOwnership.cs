using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamOwnership : MonoBehaviour
{
    public uint playerID;
    
    //set up collision layers and colours
    void Init(Color colour, LayerMask mask)
    {
        Transform[] transforms = transform.GetComponentsInChildren<Transform>();
        foreach (Transform trans in transforms)
        {
            if (trans.gameObject.layer == 0)
                trans.gameObject.layer = mask;

            Renderer renderer = trans.GetComponent<Renderer>();
            if (renderer != null)
            { 
                Material mat = renderer.material;
                mat.SetColor("Team Colour", colour);
            }
        }
    }
}
