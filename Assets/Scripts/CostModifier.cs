using System.Collections.Generic;
using UnityEngine;
using RTSModularSystem;
using RTSModularSystem.GameResources;

public class CostModifier : MonoBehaviour
{
    public static CostModifier instance { get; private set; }
    static float globalCostModifier = 1.0f;
    static Dictionary<GameActionData, float> actionCostModifiers = new Dictionary<GameActionData, float>();

    static List<ResourceQuantity> modifiedQuantities = new List<ResourceQuantity>();

    private void Start()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }


    public static float GetCostModifier(GameActionData data)
    {
        if (actionCostModifiers.ContainsKey(data))
            return actionCostModifiers[data] * globalCostModifier;
        else
            return globalCostModifier;
    }

    
    public static List<ResourceQuantity> GetModifiedCost(GameActionData data)
    {
        modifiedQuantities.Clear();

        float modifier = 1.0f;
        if (actionCostModifiers.ContainsKey(data))
            modifier = actionCostModifiers[data] * globalCostModifier;
        else
            return data.resourceChange;

        for (int i = 0; i < data.resourceChange.Count; i++)
        {
            ResourceQuantity temp = data.resourceChange[i];
            temp.quantity = Mathf.RoundToInt(temp.quantity * modifier);
            modifiedQuantities.Add(temp);
        }

        return modifiedQuantities;
    }


    public static void SetActionModifier(GameActionData data, float modifier)
    {
        if (actionCostModifiers.ContainsKey(data))
            actionCostModifiers[data] = modifier;
        else
            actionCostModifiers.Add(data, modifier);
    }


    public static void AddToActionModifier(GameActionData data, float modifierAddition)
    {
        if (actionCostModifiers.ContainsKey(data))
            actionCostModifiers[data] += modifierAddition;
        else
            actionCostModifiers.Add(data, 1.0f + modifierAddition);
    }


    public static void SetGlobalModifier(float modifier)
    {
        globalCostModifier = modifier;
    }


    public static void AddToGlobalModifier(float modifierAddition)
    {
        globalCostModifier += modifierAddition;
    }
}
