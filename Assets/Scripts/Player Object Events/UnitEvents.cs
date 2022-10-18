using UnityEngine;

public class UnitEvents : EffectEventsBase
{
    #region genericUnit

    public void GenericUnitMoveBegin(GameObject go)
    {
        if (GameData.instance.isHost)
        {
            SetAnimationBool(go, "IsMoving", true);
        }
    }


    public void GenericUnitMoveEnd(GameObject go)
    {
        if (GameData.instance.isHost)
        {
            SetAnimationBool(go, "IsMoving", false);
        }
    }


    public void GenericUnitAttack(GameObject go)
    {
        if (GameData.instance.isHost)
        {
            SetAnimationTrigger(go, "Attacking");
        }
    }

    #endregion

    #region engineer

    public void EngineerBuildBegin(GameObject go)
    {
        if (GameData.instance.isHost)
        {
            SetAnimationBool(go, "IsBuilding", true);
            SetAnimationBool(go, "IsMoving", false);
        }
    }


    public void EngineerBuildEnd(GameObject go)
    {
        if (GameData.instance.isHost)
        {
            SetAnimationBool(go, "IsBuilding", false);
        }
    }

    #endregion
}
