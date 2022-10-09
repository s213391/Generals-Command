using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOwnership : MonoBehaviour
{
    public int owningPlayer { get; private set; }
    
    //set up collision layers and colours
    void Init(int playerNumber)
    {
        owningPlayer = playerNumber;

        LayerMask mask = LayerMask.NameToLayer("Team " + owningPlayer.ToString());
        Color colour = GameData.instance.playerData[owningPlayer].colour;
        
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
