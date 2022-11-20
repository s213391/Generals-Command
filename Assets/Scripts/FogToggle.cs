using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogToggle : MonoBehaviour
{
    public void Foggle(bool on)
    {
        RenderSettings.fog = on;
    }
}
