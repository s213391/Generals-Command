using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSModularSystem;
using TMPro;

public class MissilePrice : MonoBehaviour
{
    public GameActionData nukeSpawnData;
    public TextMeshProUGUI text;

    // Update is called once per frame
    void Update()
    {
        text.text = $"{Mathf.CeilToInt(100 * CostModifier.GetCostModifier(nukeSpawnData))}%";
    }
}
