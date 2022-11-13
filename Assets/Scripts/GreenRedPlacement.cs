using UnityEngine;
using RTSModularSystem;

public class GreenRedPlacement : MonoBehaviour
{
    public void ConditionalColour(ConditionEventData data)
    {
        Color colour;
        if (data.success)
            colour = Color.green;
        else
            colour = Color.red;

        Renderer renderer = data.firstSpawnedObject.GetComponentInChildren<Renderer>();
        Material mat = renderer.material;
        mat.color = colour;
        renderer.material = mat;
    }


    public void ShowEngineerRange(PlayerObject po, GameActionData data)
    {
        if (data.successConditions[0].type == ActionConditionType.proximityToObjects)
        {
            float range = data.successConditions[0].distance;

            po.transform.GetChild(4).localScale = new Vector3(range, range, 20);
            po.transform.GetChild(4).gameObject.SetActive(true);
        }
    }


    public void HideEngineerRange(PlayerObject po, GameActionData data)
    {
        po.transform.GetChild(4)?.gameObject.SetActive(false);
    }
}
