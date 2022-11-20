using UnityEngine;
using RTSModularSystem;

public class ShowUnitProduction : MonoBehaviour
{
    public void Icon(PlayerObject po, GameActionData gad)
    {
        CurrentlyBuildingIcon cbi = po.gameObject.GetComponentInChildren<CurrentlyBuildingIcon>();
        if (cbi)
            cbi.NewProduction(gad.icon, ProductionModifier.GetModifiedDuration(gad));
    }
}
