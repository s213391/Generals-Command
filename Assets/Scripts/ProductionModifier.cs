using System.Collections.Generic;
using UnityEngine;
using RTSModularSystem;

public class ProductionModifier : MonoBehaviour
{
    public static ProductionModifier instance { get; private set; }
    static float globalProductionModifier = 1.0f;
    static Dictionary<GameActionData, float> actionProductionModifiers = new Dictionary<GameActionData, float>();


    private void Start()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }


    public static float GetProductionModifier(GameActionData data)
    {
        if (actionProductionModifiers.ContainsKey(data))
            return actionProductionModifiers[data] * globalProductionModifier;
        else
            return globalProductionModifier;
    }


    public static float GetModifiedDuration(GameActionData data)
    {
        float modifier = 1.0f;
        if (actionProductionModifiers.ContainsKey(data))
            modifier = actionProductionModifiers[data] * globalProductionModifier;

        foreach (ActionEnd ae in data.endConditions)
            if (ae.type == ActionEndType.duration)
                return ae.seconds * modifier;

        return 0.0f;
    }


    public static void SetActionModifier(GameActionData data, float modifier)
    {
        if (actionProductionModifiers.ContainsKey(data))
            actionProductionModifiers[data] = modifier;
        else
            actionProductionModifiers.Add(data, modifier);
    }


    public static void AddToActionModifier(GameActionData data, float modifierAddition)
    {
        if (actionProductionModifiers.ContainsKey(data))
            actionProductionModifiers[data] += modifierAddition;
        else
            actionProductionModifiers.Add(data, 1.0f + modifierAddition);
    }


    public static void SetGlobalModifier(float modifier)
    {
        globalProductionModifier = modifier;
    }


    public static void AddToGlobalModifier(float modifierAddition)
    {
        globalProductionModifier += modifierAddition;
    }
}
