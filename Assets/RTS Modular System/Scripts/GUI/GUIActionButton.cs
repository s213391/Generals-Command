using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTSModularSystem.GameResources;
using RTSModularSystem;
using TMPro;

public class GUIActionButton : MonoBehaviour
{
    private GameActionData action;
    private List<ResourceQuantity> currentResources;
    private Button button;
    private uint ID;
    public List<TextMeshProUGUI> costTexts;
    
    public void Init(GameActionData data)
    {
        action = data;
        button = GetComponent<Button>();
        ID = RTSPlayer.GetID();
    }
    
    
    public void OnUpdate()
    {
        /*currentResources = ResourceManager.instance.GetResourceValues(ID, ID);

        for (int i = 0; i < costTexts.Count; i++)
        {
            bool found = false;
            foreach (ResourceQuantity cost in CostModifier.GetModifiedCost(action))
            {
                if ((int)cost.resourceType == i)
                {
                    costTexts[i].text = (-cost.quantity).ToString();
                    found = true;
                    if (currentResources[i].quantity > -cost.quantity)
                        costTexts[i].color = Color.white;
                    else
                        costTexts[i].color = Color.red;

                    break;
                }
            }

            if (!found)
            {
                costTexts[i].text = "0";
                costTexts[i].color = Color.white;
            }
        }*/
    }
}
