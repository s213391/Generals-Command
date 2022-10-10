using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSModularSystem;

public class CommandCentreEvents : MonoBehaviour
{
    public void Damage(GameObject commandCentre, int newHealth, int oldHealth)
    {
        GUIPlayerScore.instance.UpdateHealth((int)(commandCentre.GetComponent<PlayerObject>().owningPlayer), (newHealth / 10));
    }


    public void Heal(GameObject commandCentre, int newHealth, int oldHealth)
    {

    }


    public void Death(GameObject commandCentre)
    {

    }
}
