using System.Collections;
using UnityEngine;
using RTSModularSystem;

public class SuddenDeath : MonoBehaviour
{
    static SuddenDeath instance;

    [SerializeField, Range(0,1)]
    float finalCostPercentage = 0.5f;
    [SerializeField, Range(0, 1)]
    float finalBuildTimePercentage = 0.5f;
    [SerializeField]
    int totalReductions = 5;
    [SerializeField]
    float secondsBetweenReductions = 10.0f;
    [SerializeField]
    GameActionData buildNuclearMissile;

    static bool suddenDeathStarted = false;
    float currentCostPercentage = 1.0f;
    float currentBuildTimePercentage = 1.0f;


    private void Start()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }


    public static void Begin()
    {
        if (!suddenDeathStarted)
            instance.StartCoroutine(instance.ReduceMissileCost());
    }


    IEnumerator ReduceMissileCost()
    {
        suddenDeathStarted = true;

        for (int i = 1; i <= totalReductions; i++)
        {
            currentCostPercentage = Mathf.Lerp(1.0f, finalCostPercentage, (float)i / (float)totalReductions);
            CostModifier.SetActionModifier(buildNuclearMissile, currentCostPercentage);
            currentBuildTimePercentage = Mathf.Lerp(1.0f, finalBuildTimePercentage, (float)i / (float)totalReductions);
            ProductionModifier.SetActionModifier(buildNuclearMissile, currentBuildTimePercentage);



            yield return new WaitForSeconds(secondsBetweenReductions);
        }
    }
}
