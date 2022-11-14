using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRotater : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (GameData.instance.isHost)
            transform.RotateAround(transform.position, Vector3.up, 180.0f);
    }
}
