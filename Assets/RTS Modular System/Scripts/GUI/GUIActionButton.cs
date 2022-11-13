using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTSModularSystem.GameResources;
using RTSModularSystem;

public class GUIActionButton : MonoBehaviour
{
    private List<ResourceQuantity> cost;
    private Button button;
    private uint ID;
    
    public void Init(List<ResourceQuantity> costs)
    {
        cost = costs;
        button = GetComponent<Button>();
        ID = RTSPlayer.GetID();
    }
    
    
    public void OnUpdate()
    {
        if (ResourceManager.instance.IsResourceChangeValid(ID, ID, cost, false, true))
            button.interactable = true;
        else
            button.interactable = false;
    }
}
