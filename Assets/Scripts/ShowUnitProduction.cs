using UnityEngine;
using RTSModularSystem;

public class ShowUnitProduction : MonoBehaviour
{
    public void Icon(PlayerObject po, GameActionData gad)
    {
        CurrentlyBuildingIcon cbi = po.gameObject.GetComponentInChildren<CurrentlyBuildingIcon>();
        if (cbi)
        {
            float duration = 0.0f;
            for (int i = 0; i < gad.endConditions.Count; i++)
            {
                if (gad.endConditions[i].type == ActionEndType.duration)
                {
                    duration = gad.endConditions[i].seconds;
                    break;
                }
            }

            cbi.NewProduction(gad.icon, duration);
        }
    }
}
