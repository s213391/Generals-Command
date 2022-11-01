using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSModularSystem;

public class BuildMode : MonoBehaviour
{
    public void BuildingPreview(Vector3 screenPointWorldSpace, PlayerObject objectUnderScreenPoint)
    {
        //if touching the building, disable camera input, otherwise move the camera and have the building follow it
        if (objectUnderScreenPoint?.gameObject.layer == LayerMask.NameToLayer("Preview"))
        {
            CameraController.instance.ToggleCameraInputs(false);
        }
        else
        {
            CameraController.instance.ToggleCameraInputs(true);
        }
    }
}
