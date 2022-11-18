using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSModularSystem;
using RTSModularSystem.GameResources;

public class CostModifier : MonoBehaviour
{
    public static CostModifier instance { get; private set; }
    public static float globalCostModifier = 1.0f;
    public static Dictionary<GameActionData, float> actionCostModifiers = new Dictionary<GameActionData, float>();

    private void Start()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }


    public static float GetModifiedCost(GameActionData data)
    {

    }


    public static void SetActionModifier(GameActionData data, float modifier)
    {

    }


    public static void AddToActionModifier(GameActionData data, float modifierAddition)
    {

    }


    public static void SetGlobalModifier(float modifier)
    {

    }


    public static void AddToGlobalModifier(float modifierAddition)
    {

    }
}
