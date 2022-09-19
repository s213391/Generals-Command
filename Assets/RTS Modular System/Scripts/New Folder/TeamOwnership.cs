using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamOwnership : MonoBehaviour
{
    public int playerID { get; private set; }
    
    //set up collision layers and colours
    void Init(int playerNumber)
    {
        playerID = playerNumber;

        LayerMask mask = LayerMask.NameToLayer("Team " + playerID.ToString());
        Color colour = GameData.instance.playerInfo[playerID].colour;
        
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
