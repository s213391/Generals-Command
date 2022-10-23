using UnityEngine;
using RTSModularSystem;

public class CommandCenterEvents : EffectEventsBase
{
    public void Damage(GameObject commandCentre, int newHealth, int oldHealth)
    {
        GUIPlayerScore.instance.UpdateHealth((int)(commandCentre.GetComponent<PlayerObject>().owningPlayer - 1), (newHealth / 10));
    }


    public void Heal(GameObject commandCentre, int newHealth, int oldHealth)
    {

    }


    public void Death(GameObject commandCentre)
    {
        if (commandCentre.GetComponent<PlayerObject>().owningPlayer == GameData.instance.localPlayerNumber + 1)
            GameOver.instance.TriggerGameOver(false);
        else
            GameOver.instance.TriggerGameOver(true);
    }
}
