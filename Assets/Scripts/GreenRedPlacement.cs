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
}
