using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSModularSystem
{
    public class CameraController : MonoBehaviour
    {
        [Header("Position")]
        [Tooltip("Target object for the camera, and what the camera controller movement options actually move.\nCan be swapped to change the camera view between different positions and even follow moving objects")]
        public GameObject target;
        [Tooltip("Whether the target can be moved by the camera controller, or if the camera will simply follow passively")]
        public bool canMoveTarget = true;
        [Tooltip("Default distance camera sits behind the target in metres")]
        public float defaultDistance = 5.0f;
        [Tooltip("The X-axis angle the camera faces down at, recommended between 30 and 90")]
        public float xAngle = 45.0f;

        [Header("Zoom")]
        public float zoomSpeed = 2.0f;
        [Min(0.1f)] [Tooltip("The minimum distance from target object the camera can be. \nIncrease if camera clips through ground")]
        public float minZoom = 2.0f;
        [Tooltip("The maximum distance from target object the camera can be. \nDecrease if world disappears or mouse tracking stops working")]
        public float maxZoom = 10.0f;
        [Tooltip("How much of an effect that zooming out has on speeding up camera movement. \n0 or less will disable this feature")]
        public float speedUpOnZoomOut = 0.2f;

        [Header("Movement")]
        public float movementSpeed = 0.7f;
        [Tooltip("Uses Horizontal and Vertical Axes set in Input Manager.\nIncludes WASD, Arrows and Joysticks by default.")]
        public bool moveWithInputManager;
        [Tooltip("Uses the mouse position as well as a safe zone and a read zone to determine how fast to move the camera.")]
        public bool moveWithMouse;

        [Tooltip("The percentage width of the screen that moving the mouse in won't move the camera sideways. \nValue of 1 will only move camera if mouse is outside of window")]
        [Range(0f, 1f)]
        public float xSafeZone = 0.6f;
        [Tooltip("The percentage height of the screen that moving the mouse in won't move the camera up or down. \nValue of 1 will only move camera if mouse is outside of window")]
        [Range(0f, 1f)]
        public float ySafeZone = 0.8f;
        [Tooltip("The percentage of the screen width that the mouse position will be tracked in.\nValues above 1 allow tracking outside of window")]
        [Range(0f, 1.5f)]
        public float xReadZone = 1.0f;
        [Tooltip("The percentage of the screen heigth that the mouse position will be tracked in.\nValues above 1 allow tracking outside of window")]
        [Range(0f, 1.5f)]
        public float yReadZone = 1.0f;
        [Tooltip("Whether moving the mouse outside of the read zone is considered movement at maximum speed(true) or no movement(false)")]
        public bool moveWhenOutsideOfReadZone;

        private float currentDistance; //the current distance camera sits behind the target in unity units

        //set the camera's initial position and angle
        public void Init(bool isHost)
        {
            currentDistance = defaultDistance;
            transform.eulerAngles = new Vector3(xAngle, 0.0f, 0.0f);
            canMoveTarget = true;

            //set target's initial position based on host status
            if (isHost)
                target.transform.position = ObjectDataManager.HostPosition();
            else
                target.transform.position = ObjectDataManager.ClientPosition();

            //prevent camera moving in opposite direction
            if (xSafeZone >= xReadZone || ySafeZone >= yReadZone)
            {
                Debug.Log("Camera Controller: The mouse safe zone cannot be larger than the mouse read zone, Destroying controller");
                Destroy(this);
            }
        }

        //move the target and update the cameras position
        public void OnUpdate()
        {
            if (canMoveTarget)
            {
                float dx = 0.0f;
                float dz = 0.0f;

                //get the camera movement based on mouse position
                if (moveWithMouse)
                {
                    Vector3 mousePos = Input.mousePosition;
                    //offset the mouse position so that the centre of the screen is (0,0) and pos is between -1 and 1
                    mousePos.x -= Screen.width / 2;
                    mousePos.x /= Screen.width / 2;
                    mousePos.y -= Screen.height / 2;
                    mousePos.y /= Screen.height / 2;

                    //get the percentage value of how far out of the safe zone the mouse is horizontally
                    if (Mathf.Abs(mousePos.x) > xReadZone)
                    {
                        //mouse is outside of readzone, follow settings
                        if (moveWhenOutsideOfReadZone)
                        {
                            if (mousePos.x > 0)
                                dx += 1.0f;
                            else
                                dx -= 1.0f;
                        }
                    }
                    else if (Mathf.Abs(mousePos.x) > xSafeZone)
                    {
                        //mouse is in readzone but out of safe zone, set camera movement based on depth into non-safe readzone
                        if (mousePos.x > 0.0f)
                            mousePos.x -= xSafeZone;
                        else
                            mousePos.x += xSafeZone;
                        dx += mousePos.x / (xReadZone - xSafeZone);
                    }


                    //get the percentage value of how far out of the safe zone the mouse is vertically
                    if (Mathf.Abs(mousePos.y) > yReadZone)
                    {
                        //mouse is outside of readzone, follow settings
                        if (moveWhenOutsideOfReadZone)
                            if (moveWhenOutsideOfReadZone)
                            {
                                if (mousePos.y > 0)
                                    dz += 1.0f;
                                else
                                    dz -= 1.0f;
                            }
                    }
                    else if (Mathf.Abs(mousePos.y) > ySafeZone)
                    {
                        //mouse is in readzone but out of safe zone, set camera movement based on depth into non-safe readzone
                        if (mousePos.y > 0.0f)
                            mousePos.y -= ySafeZone;
                        else
                            mousePos.y += ySafeZone;
                        dz += mousePos.y / (yReadZone - ySafeZone);
                    }

                    //make sure mouse movements do not affect camera movement outside of readzone if not desired
                    if (!moveWhenOutsideOfReadZone && (Mathf.Abs(mousePos.x) > xReadZone || Mathf.Abs(mousePos.y) > yReadZone))
                    {
                        dx = 0.0f;
                        dz = 0.0f;
                    }
                }

                //get camera movement based on input manager
                if (moveWithInputManager)
                {
                    dx += Input.GetAxisRaw("Horizontal");
                    dz += Input.GetAxisRaw("Vertical");
                }

                //clamp camera movement and update target position
                dx = Mathf.Clamp(dx, -1, 1);
                dz = Mathf.Clamp(dz, -1, 1);

                Transform targetTrans = target.transform;

                if (speedUpOnZoomOut > 0)
                    targetTrans.position += new Vector3(dx, 0.0f, dz) * movementSpeed * Time.deltaTime * currentDistance * speedUpOnZoomOut;
                else
                    targetTrans.position += new Vector3(dx, 0.0f, dz) * movementSpeed * Time.deltaTime;
            }

            // zoom in/out with mouse wheel 
            currentDistance = Mathf.Clamp(currentDistance - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, minZoom, maxZoom);

            // look at the target point 
            transform.position = target.transform.position - currentDistance * transform.forward;
        }
    }
}
