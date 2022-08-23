using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using DS_Selection;

using Selectable = DS_Selection.Selectable;

namespace RTSModularSystem
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UnitArrangement))]
    public class PlayerInput : MonoBehaviour
    {
        public static PlayerInput instance;
        
        [SerializeField]
        private RectTransform selectionBox;
        [SerializeField]
        private LayerMask objectLayers;
        [SerializeField]
        private LayerMask terrainLayers;
        [SerializeField]
        private float dragDelay = 0.1f;

        private Camera mainCam;
        private SelectionController selectionController;
        private UnitArrangement unitArrangement;
        private Image selectionImage;
        private float downTime;
        private Vector2 originalPos;
        private Vector3 nullState = new Vector3(-99999, -99999, -99999);
        private DeviceType device;
        private bool selectionEnabled = true;

        public Vector3 screenPointWorldSpace { get; private set; }
        public PlayerObject objectUnderScreenPoint { get; private set; }
        public Ray screenRay { get; private set; }


        //set up singleton
        void Start()
        {
            if (instance != null && instance != this)
                Destroy(this);
            else
                instance = this;
        }


        //set up all variables and gameobjects required for getting player input
        public void Init()
        {
            mainCam = Camera.main;

            //set up object selection and movement orders
            if (!selectionBox)
                selectionBox = GameObject.FindGameObjectWithTag("Selection Box").GetComponent<RectTransform>();
            selectionImage = selectionBox.GetComponent<Image>();
            selectionImage.enabled = false;
            selectionController = SelectionController.instance;
            unitArrangement = GetComponent<UnitArrangement>();

            device = SystemInfo.deviceType;
            if (device == DeviceType.Handheld)
                selectionEnabled = false;
        }


        //read and handle global player input while publicly exposing values for other scripts to use this frame
        public void OnUpdate()
        {
            if (device == DeviceType.Desktop)
            {
                CheckUnderScreenPoint();
                HandleSelectionInputs();
                HandleMovementInputs();
            }
            else if (device == DeviceType.Handheld)
            {
                //only check if the screen is being touched with one finger
                if (Input.touchCount == 1)
                {
                    CheckUnderScreenPoint();
                    HandleSelectionInputs();
                    HandleMovementInputs();
                }
                else
                {
                    objectUnderScreenPoint = null;
                }
            }
        }


        //get information involving the mouse and anything under it
        private void CheckUnderScreenPoint()
        {
            if (device == DeviceType.Desktop)
                screenRay = mainCam.ScreenPointToRay(Input.mousePosition);
            else if (device == DeviceType.Handheld)
                screenRay = mainCam.ScreenPointToRay(Input.GetTouch(0).position);

            //check the terrain to get the world space position of the mouse
            if (Physics.Raycast(screenRay, out RaycastHit terrainHit, 250.0f, terrainLayers))
                screenPointWorldSpace = terrainHit.point;
            else
                screenPointWorldSpace = nullState;

            //check player objects to see if any are under the mouse
            if (Physics.Raycast(screenRay, out RaycastHit objectHit, 250.0f, objectLayers))
                objectUnderScreenPoint = objectHit.collider.GetComponentInParent<PlayerObject>();
            else
                objectUnderScreenPoint = null;
        }


        //handle the selection of player objects, either through single click or drag
        private void HandleSelectionInputs()
        {
            if (!selectionEnabled)
                return;
            
            //track if the touch/click is up, down or being held
            bool down = false;
            bool held = false;
            bool up = false;

            if (device == DeviceType.Desktop)
            {
                down = Input.GetKeyDown(KeyCode.Mouse0);
                held = Input.GetKey(KeyCode.Mouse0);
                up = Input.GetKeyUp(KeyCode.Mouse0);
            }
            else if (device == DeviceType.Handheld)
            {
                Touch touch = Input.GetTouch(0);
                down = touch.phase == TouchPhase.Began;
                held = touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary;
                up = touch.phase == TouchPhase.Ended;
            }
            
            //if the touch/click began this frame, set initial values
            if (down)
            {
                selectionBox.sizeDelta = Vector2.zero;
                selectionImage.enabled = true;
                downTime = 0.0f;

                if (device == DeviceType.Desktop)
                    originalPos = Input.mousePosition;
                else if (device == DeviceType.Handheld)
                    originalPos = Input.GetTouch(0).rawPosition;
            }
            //if the touch/click is still down, update timer and selection box
            else if (held)
            {
                downTime += Time.deltaTime;
                if (downTime >= dragDelay)
                {
                    float width = 0.0f;
                    float height = 0.0f;

                    if (device == DeviceType.Desktop)
                    {
                        width = Input.mousePosition.x - originalPos.x;
                        height = Input.mousePosition.y - originalPos.y;
                    }
                    else if (device == DeviceType.Handheld)
                    {
                        Touch touch = Input.GetTouch(0);
                        width = touch.position.x - originalPos.x;
                        height = touch.position.y - originalPos.y;
                    }

                    selectionBox.anchoredPosition = originalPos + new Vector2(width / 2, height / 2);
                    selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
                }
            }
            //if the touch/click ended this frame, check if anything has been selected
            else if (up)
            {
                //only check the shift key mechanic on desktop
                bool shift = false;
                if (device == DeviceType.Desktop) 
                    shift = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));

                //used to shorten the long name of this list
                List<Selectable> selectables = selectionController.availableObjects;

                //only check the box if the touch/click has been down for a set delay
                if (downTime > dragDelay)
                {
                    //create a bounding box in screen space and select every owned player object in the world space of that box
                    Bounds bounds = new Bounds(selectionBox.anchoredPosition, selectionBox.sizeDelta);
                    for (int i = 0; i < selectables.Count; i++)
                    {
                        PlayerObject po = selectables[i].GetComponent<PlayerObject>();
                        if (po && RTSPlayer.Owns(po) && IsInSelectionBox(po, bounds))
                        {
                            if (selectionController.IsSelected(selectables[i]) && shift)
                                selectionController.Deselect(selectables[i]);
                            else
                                selectionController.Select(selectables[i]);
                        } 
                        else if (!shift)
                            selectionController.Deselect(selectables[i]);
                    }
                }
                //else treat as a single click
                else 
                {
                    if (objectUnderScreenPoint != null && RTSPlayer.Owns(objectUnderScreenPoint))
                    {
                        Selectable selectable = objectUnderScreenPoint.GetComponent<Selectable>();

                        if (shift)
                        {
                            if (selectionController.IsSelected(selectable))
                                selectionController.Deselect(selectable);
                            else
                                selectionController.Select(selectable);
                        }
                        else
                        {
                            selectionController.DeselectAll();
                            selectionController.Select(selectable);
                        }
                    }
                    else if (!shift)
                        selectionController.DeselectAll();
                }

                //hide the selection box
                selectionBox.sizeDelta = Vector2.zero;
                selectionImage.enabled = false;

                downTime = 0.0f;
            }
        }


        //check if any selected objects need to be given a navmesh destination
        private void HandleMovementInputs()
        {
            if (!selectionEnabled || selectionController.selectedObjects.Count == 0 || screenPointWorldSpace == nullState)
                return;
            if (device == DeviceType.Desktop && !Input.GetKeyUp(KeyCode.Mouse1))
                return;

            //add every selected movable object to a list
            List<NavMeshAgent> moveables = new List<NavMeshAgent>();
            foreach (Selectable selectable in selectionController.selectedObjects)
            {
                PlayerObject po = selectable.GetComponent<PlayerObject>();
                if (po && po.data.moveable)
                    moveables.Add(po.GetComponent<NavMeshAgent>());
            }

            //check if an object is already at the clicked point and set it as the movement target
            if (objectUnderScreenPoint == null)
                unitArrangement.AssignDestination(moveables, screenPointWorldSpace);
            else if (RTSPlayer.Owns(objectUnderScreenPoint))
                unitArrangement.AssignDestination(moveables, objectUnderScreenPoint.transform.position, false, objectUnderScreenPoint);
            else
                unitArrangement.AssignDestination(moveables, objectUnderScreenPoint.transform.position, true, objectUnderScreenPoint);
        }


        //returns whether the given player object is within the given bounds
        private bool IsInSelectionBox(PlayerObject po, Bounds bounds)
        {
            Vector3 pos = mainCam.WorldToScreenPoint(po.transform.position);

            return pos.x > bounds.min.x && pos.x < bounds.max.x
                && pos.y > bounds.min.y && pos.y < bounds.max.y;
        }


        //enables/disables selection and movement
        public void ToggleSelectionInputs(bool enabled)
        {
            selectionEnabled = enabled;
        }
    }
}