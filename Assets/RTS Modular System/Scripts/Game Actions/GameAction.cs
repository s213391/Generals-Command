using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using RTSModularSystem.GameResources;

namespace RTSModularSystem
{
    [DisallowMultipleComponent]
    //attached to the player prefab, handles all the actions performed by buildings, units and the player
    public class GameAction : NetworkBehaviour
    {
        List<ActionTrigger> triggers = new List<ActionTrigger>();
        
        //check if action can be started, then start it
        public void StartAction(GameActionData actionData, PlayerObject functionCaller, uint owningPlayer)
        {
            //hypothetically this should never occur, but better to be safe
            if (!isLocalPlayer)
                return;

            actionData.onActionStart?.Invoke(functionCaller, actionData);

            if (actionData.clientSide)
            {
                //use empty data, it won't be checked
                NetworkInputData inputData = new NetworkInputData { mouseRay = new Ray() };
                StartCoroutine(PerformAction(actionData, functionCaller.gameObject, inputData, owningPlayer));
                functionCaller.SetInterrupt(1);
            }
            else
            {
                //make sure the action data requested does exist and is stored properly before sending across the network
                NetworkActionData networkData = DataToNetwork(actionData, functionCaller);
                if (networkData.objectIndex == -1 || networkData.actionIndex == -1)
                {
                    Debug.LogError("Action has invalid indexes, can not be sent over network");
                    return;
                }

                //get the clients active inputs
                NetworkInputData inputData = GetInputData();

                //send the action to the server to carry out
                RTSPlayer.localPlayer.CmdStartAction(networkData, functionCaller.gameObject, inputData, owningPlayer);
            }
        }


        //exact same as regular action, just run on the server
        [Server]
        public void StartServerAction(NetworkActionData networkData, GameObject functionCaller, NetworkInputData inputData, uint owningPlayer)
        {
            GameActionData actionData = NetworkToData(networkData, functionCaller);
            PlayerObject playerObject = functionCaller.GetComponent<PlayerObject>();

            StartCoroutine(PerformAction(actionData, functionCaller, inputData, owningPlayer));
            playerObject.SetInterrupt(1);
        }


        //converts action data to network action data
        public NetworkActionData DataToNetwork(GameActionData actionData, PlayerObject playerObject)
        {
            //set invalid values initially in case of error
            NetworkActionData networkData = new NetworkActionData { objectType = PlayerObjectType.other, objectIndex = -1, actionIndex = -1, owningPlayer = 9999 };

            //get the player object data with error checking
            PlayerObjectData playerObjectData = playerObject.data;
            if (playerObjectData == null)
            {
                Debug.LogError("DtN: PlayerObject has no PlayerObjectData, can not read actions");
                return networkData;
            }

            //player object data exists, get indexes
            networkData.owningPlayer = RTSPlayer.GetID();
            networkData.objectType = playerObjectData.objectType;
            networkData.objectIndex = ObjectDataManager.instance.GetObjectIndex(playerObjectData);
            networkData.actionIndex = playerObject.GetActionIndex(actionData);
            return networkData;
        }


        //converts network action data to action data
        public GameActionData NetworkToData(NetworkActionData networkData, GameObject gameObject)
        {
            //get the player object data with error checking
            PlayerObject playerObject = gameObject.GetComponent<PlayerObject>();
            if (playerObject == null)
            {
                Debug.LogError("NtD: GameObject has no PlayerObject component, can not read actions");
                return null;
            }
            PlayerObjectData playerObjectData = playerObject.data;
            if (playerObjectData == null)
            {
                Debug.LogError("NtD: PlayerObject has no PlayerObjectData, can not read actions");
                return null;
            }

            //player object data exists, check it matches and get action data
            if (playerObjectData.name != ObjectDataManager.instance.GetObjectData(networkData.objectType, networkData.objectIndex).name)
            {
                Debug.LogError("NtD: PlayerObjectData does not match networkData");
                return null;
            }
            if (playerObjectData.actions.Count <= networkData.actionIndex)
            {
                Debug.LogError("NtD: Client has different PlayerObjectData to server, check game version");
                return null;
            }

            //data passed all error checks
            return playerObjectData.actions[networkData.actionIndex].action;
        }


        //get active inputs
        public NetworkInputData GetInputData()
        {
            NetworkInputData inputData = new NetworkInputData();

            inputData.mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            return inputData;
        }


        //interrupts certain actions started by the given player object
        void InterruptActions(GameObject go, int value)
        {
            go.GetComponent<PlayerObject>().SetInterrupt(value);
        }


        [Server]
        //initialise PlayerObject on newly spawned gameobject
        void InitialiseNewPlayerObject(GameObject newObject, uint owningPlayer)
        {
            newObject.GetComponent<PlayerObject>().SetID(owningPlayer);
        }


        //sets a trigger to true if it exists in the list
        public void SetTrigger(PlayerObject po, GameActionData data, bool successful)
        {
            if (po == null || data == null)
                return;

            for (int i = 0; i < triggers.Count; i++)
            {
                if (triggers[i].objectPerformingAction == po && triggers[i].actionBeingPerformed == data)
                {
                    ActionTrigger newTrigger = triggers[i];
                    newTrigger.successful = successful;
                    newTrigger.triggered = true;
                    triggers[i] = newTrigger;
                    return;
                }
            }
        }


        //returns the first end condition that is active this frame, defaults to none condition
        private ActionEnd EndConditionActive(List<ActionEnd> endConditions, float duration, PlayerObject functionCaller, GameActionData data)
        {
            foreach (ActionEnd ae in endConditions)
            {
                switch (ae.type)
                {
                    case ActionEndType.leftClick:
                        if (Input.GetMouseButtonUp(0))
                            return ae;
                        break;

                    case ActionEndType.rightClick:
                        if (Input.GetMouseButtonUp(1))
                            return ae;
                        break;

                    case ActionEndType.enter:
                        if (Input.GetKeyDown(KeyCode.KeypadEnter))
                            return ae;
                        break;

                    case ActionEndType.escape:
                        if (Input.GetKeyDown(KeyCode.Escape))
                            return ae;
                        break;

                    case ActionEndType.immediate:
                        return ae;

                    case ActionEndType.duration:
                        if (duration >= ae.seconds)
                            return ae;
                        break;

                    case ActionEndType.trigger:
                        for (int i = 0; i < triggers.Count; i++)
                        {
                            if (triggers[i].objectPerformingAction == functionCaller && triggers[i].actionBeingPerformed == data)
                            {
                                if (triggers[i].triggered)
                                {
                                    ActionEnd temp = ae;
                                    temp.successfulEnd = triggers[i].successful;
                                    triggers.RemoveAt(i);
                                    return temp;
                                }
                                else
                                    break;
                            }
                        }
                        break;
                }
            }

            //no conditions met, return none
            ActionEnd none = new ActionEnd();
            none.type = ActionEndType.none;
            none.successfulEnd = false;
            return none;
        }


        //evaluates all success conditions and returns whether all conditions passed
        private bool EvaluateSuccess(GameActionData data, PlayerObject functionCaller, GameObject firstObjectSpawned)
        {
            bool success = true;
            List<ActionCondition> conditions = data.successConditions;

            for (int i = 0; i < conditions.Count; i++)
            {
                ActionCondition ac = conditions[i];
                ac.conditionMet = false;

                GameObject objectToCheck;
                if (ac.objectToBeChecked == ObjectToBeChecked.self)
                    objectToCheck = functionCaller.gameObject;
                else
                    objectToCheck = firstObjectSpawned;

                switch (conditions[i].type)
                {
                    case ActionConditionType.proximityToObjects:
                        List<PlayerObject> objectsToBeCheckedForProximity = ObjectDataManager.GetPlayerObjectsOfType(conditions[i].objectsType.name);

                        foreach (PlayerObject po in objectsToBeCheckedForProximity)
                        {
                            if (po.owningPlayer == functionCaller.owningPlayer)
                            {
                                if ((po.transform.position - objectToCheck.transform.position).magnitude <= ac.distance)
                                {
                                    ac.conditionMet = true;
                                    break;
                                }
                            }
                        }
                        break;

                    case ActionConditionType.collidingWithLayers:
                        Bounds bounds = objectToCheck.GetComponentInChildren<Renderer>().bounds;
                        ac.conditionMet = Physics.OverlapBox(bounds.center, bounds.extents, Quaternion.identity, ac.layers).Length > 0;
                        break;

                    case ActionConditionType.onTerrain:
                        bounds = objectToCheck.GetComponentInChildren<Renderer>().bounds;

                        List<Vector3> corners = new List<Vector3>();
                        corners.Add(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y + 0.01f, bounds.center.z + bounds.extents.z));
                        corners.Add(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y + 0.01f, bounds.center.z - bounds.extents.z));
                        corners.Add(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y + 0.01f, bounds.center.z + bounds.extents.z));
                        corners.Add(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y + 0.01f, bounds.center.z - bounds.extents.z));

                        int counter = 0;
                        for (int j = 0; j < 4; j++)
                        {
                            if (Physics.Raycast(corners[j], Vector3.down, ac.maximumHeightAllowed, PlayerInput.instance.terrainLayers))
                                counter++;
                            else
                                break;
                        }
                        if (counter == 4)
                            ac.conditionMet = true;
                        break;
                }

                //check if this counts as a success
                if (ac.successIfConditionMet && !ac.conditionMet || !ac.successIfConditionMet && ac.conditionMet)
                    success = false;
                conditions[i] = ac;
            }
            ConditionEventData conditionEventData = new ConditionEventData();
            conditionEventData.functionCaller = functionCaller.gameObject;
            conditionEventData.firstSpawnedObject = firstObjectSpawned;
            conditionEventData.conditions = conditions;
            conditionEventData.success = success;

            data.onConditionEvaluate.Invoke(conditionEventData);
            return success;
        }


        [Command]
        //command to spawn a clientside object on the server
        private void CmdSpawnObjects(NetworkActionData networkData, GameObject functionCaller, uint owningPlayer, List<int> indices, List<Vector3> positions, List<Quaternion> rotations)
        {
            GameActionData actionData = NetworkToData(networkData, functionCaller);
            PlayerObject playerObject = functionCaller.GetComponent<PlayerObject>();

            for (int i = 0; i < indices.Count; i++)
            {
                GameObject gameObject = Instantiate(actionData.objectsToSpawn[indices[i]].prefab, positions[i], rotations[i]);
                NetworkServer.Spawn(gameObject);
                InitialiseNewPlayerObject(gameObject, owningPlayer);
            }
        }


        [Command]
        //command to change resources server-sider from the client
        private void CmdOneOffResourceChange(uint ID, List<ResourceQuantity> resourceChanges)
        {
            ResourceManager.instance.OneOffResourceChange(ID, ID, resourceChanges);
        }


        [Command]
        //command to change income server-side from the client
        private void CmdIncomeChange(uint ID, List<ResourceQuantity> incomeChanges)
        {
            ResourceManager.instance.IncomeChange(ID, ID, incomeChanges);
        }


        //run the action in a coroutine until interrupted or exit condition reached
        private IEnumerator PerformAction(GameActionData data, GameObject functionCaller, NetworkInputData inputData, uint owningPlayer)
        {
            if (data == null || functionCaller == null)
                yield break;
            
            PlayerObject playerObject = functionCaller.GetComponent<PlayerObject>();

            //track duration regardless of endConditions
            float duration = 0.0f;
            int durationIndex = data.endConditions.FindIndex(x => x.type == ActionEndType.duration && x.successfulEnd);

            bool previousCameraState = CameraController.instance.movementEnabled;
            bool previousDragSelectionState = PlayerInput.instance.dragSelectionEnabled;
            bool previousSingleSelectionState = PlayerInput.instance.singleSelectionEnabled;
            bool previousMovementState = PlayerInput.instance.movementEnabled;

            //set up a trigger if needed
            foreach (ActionEnd ae in data.endConditions)
            {
                if (ae.type == ActionEndType.trigger)
                {
                    triggers.Add(new ActionTrigger { objectPerformingAction = playerObject, actionBeingPerformed = data, triggered = false });
                    break;
                }
            }

            //prevent camera movement and selection during this action
            if (data.lockCamera)
                CameraController.instance.ToggleCameraInputs(false);
            if (data.lockInput)
            {
                PlayerInput.instance.ToggleDragSelectionInputs(false);
                PlayerInput.instance.ToggleSingleSelectionInputs(false);
                PlayerInput.instance.ToggleMovementInputs(false);
            }

            //allow for looping an action
            bool loop = data.loopOnSuccessfulEnd;
            do
            {
                //reset duration each loop if duration is a successful action end
                if (loop && durationIndex != -1)
                {
                    if (duration >= data.endConditions[durationIndex].seconds)
                        duration -= data.endConditions[durationIndex].seconds;
                    else
                        duration = 0.0f;
                }
                
                List<GameObject> objectsCreated = new List<GameObject>();
                List<MouseTrackingObject> objectsFollowingMouse = new List<MouseTrackingObject>();
                List<ObjectCreation> objectsToCreateAfterAction = new List<ObjectCreation>();
                List<GameObject> objectsToBeDestroyed = new List<GameObject>();
                List<GameObject> objectsToBeSpawned = new List<GameObject>();
                List<SnapPoint> snapPoints = new List<SnapPoint>();

                //instantiate all objects immediately
                foreach (ObjectCreation oc in data.objectsToSpawn)
                {
                    if (oc.createAfterAction)
                    { 
                        objectsToCreateAfterAction.Add(oc);
                        continue;
                    }
                    
                    //instantiate object in specified manner
                    GameObject prefab = null;
                    switch (oc.location)
                    {
                        case ObjectCreationLocation.atPoint:
                            Vector3 pos;
                            if (oc.worldPosition)
                                pos = oc.position;
                            else
                                pos = functionCaller.transform.position + functionCaller.transform.rotation * oc.position;

                            Quaternion rot; 
                            if (oc.worldRotation)
                                rot = Quaternion.Euler(oc.rotation);
                            else
                                rot = functionCaller.transform.rotation * Quaternion.Euler(oc.rotation);

                            prefab = Instantiate(oc.prefab, pos, rot);
                            break;

                        //create object at mouse position and add to mouse tracking list for rest of action
                        //do not instantiate anything on the server unless a valid point is given from the client up front
                        case ObjectCreationLocation.atMouse:
                            Ray ray;
                            if (data.clientSide)
                                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            else
                                ray = inputData.mouseRay;
                            RaycastHit hit;
                            Physics.Raycast(ray, out hit, 250.0f, oc.mouseLayerMask);

                            if (hit.point != null)
                                prefab = Instantiate(oc.prefab, hit.point, Quaternion.identity);
                            else if (data.clientSide)
                                prefab = Instantiate(oc.prefab, new Vector3(0, -10, 0), Quaternion.identity);

                            //set up snapping if required
                            if (oc.snapToObject)
                            {
                                string prefabName = oc.prefab.GetComponent<PlayerObject>().data.name;
                                snapPoints = SnapPoint.GetCompatibleSnapPoints(prefabName);
                                foreach (SnapPoint snapPoint in snapPoints)
                                {
                                    Transform snapTrans = snapPoint.transform;
                                    if ((snapTrans.position - hit.point).magnitude <= oc.snapDistance)
                                    {
                                        prefab.transform.position = snapTrans.position;
                                        prefab.transform.rotation = snapTrans.rotation;
                                        break;
                                    }
                                }
                            }

                            if (prefab != null && data.clientSide)
                                objectsFollowingMouse.Add(new MouseTrackingObject { obj = prefab, layerMask = oc.mouseLayerMask, snapping = oc.snapToObject, snapDistance = oc.snapDistance, onlyMoveWhenUnderCursor = oc.onlyMoveWhenUnderCursor });
                            break;

                        case ObjectCreationLocation.atObject:
                            Transform spawn = functionCaller.transform;
                            foreach (Transform child in spawn)
                            {
                                if (child.tag == "Spawnpoint")
                                {
                                    spawn = child;
                                    break;
                                }
                            }

                            prefab = Instantiate(oc.prefab, spawn.position, spawn.rotation);
                            break;
                    }
                    if (prefab != null)
                    {
                        objectsCreated.Add(prefab);
                        if (oc.destroyAfterAction)
                            objectsToBeDestroyed.Add(prefab);
                        if (oc.serverSpawnAfterAction)
                            objectsToBeSpawned.Add(prefab);
                        if (!data.clientSide)
                        {
                            //objects created serverside need to be created on all clients
                            NetworkServer.Spawn(prefab);
                            InitialiseNewPlayerObject(prefab, owningPlayer);
                        }
                        else
                        {
                            //add any clientside only objects to the preview layer
                            prefab.layer = LayerMask.NameToLayer("Preview");
                            foreach (Transform trans in prefab.GetComponentsInChildren<Transform>())
                            {
                                trans.gameObject.layer = LayerMask.NameToLayer("Preview");
                            }
                        }
                    }
                }
                    

                //with GameObjects spawned, begin the main loop of the action----------------------------------------------------------------------------------------------
                bool firstTime = true;
                while (playerObject != null && playerObject.interrupt < data.interruptionTolerance)
                {
                    //for checking success state through the action
                    bool success = true;

                    //exit conditions can be accidentally triggered by the input that started this action, delay exit checking for one frame
                    if (!firstTime)
                    {
                        duration += Time.deltaTime;

                        //get the mouse's position and update any objects following it
                        //it is impractical to send the position of the clients mouse over the network every frame, so only clientside allows mouse tracking
                        if (data.clientSide && !firstTime && objectsFollowingMouse.Count > 0)
                        {
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            RaycastHit hit;

                            //not every object will have the same LayerMask, so the raycast will need to be done for each
                            foreach (MouseTrackingObject mto in objectsFollowingMouse)
                            {

                                if (mto.onlyMoveWhenUnderCursor && SystemInfo.deviceType == DeviceType.Handheld)
                                {
                                    Physics.Raycast(ray, out hit, 250.0f, LayerMask.GetMask("Preview"));
                                    if (hit.collider == null || hit.collider.gameObject != mto.obj)
                                    {
                                        //if this uses snapping, check each snappoint to see if any are in range
                                        if (mto.snapping)
                                        {
                                            Physics.Raycast(ray, out hit, 250.0f, mto.layerMask);
                                            bool snapped = false;
                                            foreach (SnapPoint snapPoint in snapPoints)
                                            {
                                                Transform snapTrans = snapPoint.transform;
                                                if ((snapTrans.position - hit.point).magnitude <= mto.snapDistance)
                                                {
                                                    mto.obj.transform.position = snapTrans.position;
                                                    mto.obj.transform.rotation = snapTrans.rotation;
                                                    snapped = true;
                                                    break;
                                                }
                                            }
                                            //only update success if a snapping object has not snapped
                                            if (!snapped)
                                                success = false;
                                            else
                                                continue;
                                        }

                                        CameraController.instance.ToggleCameraInputs(true);
                                        mto.obj.transform.SetParent(Camera.main.transform);
                                        Physics.Raycast(mto.obj.transform.position + 50.0f * Vector3.up, Vector3.down, out hit, 100.0f, mto.layerMask);
                                        if (hit.collider != null)
                                            mto.obj.transform.position = hit.point;
                                        continue;

                                    }
                                    else
                                    {
                                        CameraController.instance.ToggleCameraInputs(false);
                                        mto.obj.transform.SetParent(null);
                                    }
                                }

                                
                                Physics.Raycast(ray, out hit, 250.0f, mto.layerMask);

                                //reset rotation every frame
                                mto.obj.transform.rotation = Quaternion.identity;


                                if (hit.collider != null)
                                {
                                    mto.obj.transform.position = hit.point;

                                    //if this uses snapping, check each snappoint to see if any are in range
                                    if (mto.snapping)
                                    {
                                        bool snapped = false;
                                        foreach (SnapPoint snapPoint in snapPoints)
                                        {
                                            Transform snapTrans = snapPoint.transform;
                                            if ((snapTrans.position - hit.point).magnitude <= mto.snapDistance)
                                            {
                                                mto.obj.transform.position = snapTrans.position;
                                                mto.obj.transform.rotation = snapTrans.rotation;
                                                snapped = true;
                                                break;
                                            }
                                        }
                                        //only update success if a snapping object has not snapped
                                        if (!snapped)
                                            success = false;
                                    }
                                }
                                else if (mto.snapping)
                                    success = false;
                            }
                        }

                        ActionEnd actionEnd = EndConditionActive(data.endConditions, duration, playerObject, data);
                        if (actionEnd.type != ActionEndType.none)
                        {
                            //evaluate conditions to choose whether this action will end as a success or a failure
                            if (success)
                                success = actionEnd.successfulEnd;
                            if (success && data.successConditions.Count > 0)
                                success = EvaluateSuccess(data, playerObject, objectsCreated[0]);

                            //for server actions, the last check for success is always resources, which will be changed now if they have not been already
                            if (success && !data.changeResourcesAtStart && data.resourceChange.Count > 0)
                            {
                                if (!data.clientSide)
                                    success = ResourceManager.instance.OneOffResourceChange(playerObject.owningPlayer, owningPlayer, data.resourceChange);
                                else if (ResourceManager.instance.IsResourceChangeValid(playerObject.owningPlayer, owningPlayer, data.resourceChange, false, true))
                                    CmdOneOffResourceChange(owningPlayer, data.resourceChange);
                                else
                                    success = false;
                            }

                            //action was successful, start success actions and spawn any required buildings server-sider
                            if (success)
                            {
                                foreach (GameActionData action in data.nextActionsOnSuccess)
                                    StartAction(action, playerObject, owningPlayer);

                                foreach (ObjectCreation oc in objectsToCreateAfterAction)
                                {
                                    //instantiate object in specified manner
                                    GameObject prefab = null;
                                    switch (oc.location)
                                    {
                                        case ObjectCreationLocation.atPoint:
                                            Vector3 pos;
                                            if (oc.worldPosition)
                                                pos = oc.position;
                                            else
                                                pos = functionCaller.transform.position + functionCaller.transform.rotation * oc.position;

                                            Quaternion rot;
                                            if (oc.worldRotation)
                                                rot = Quaternion.Euler(oc.rotation);
                                            else
                                                rot = functionCaller.transform.rotation * Quaternion.Euler(oc.rotation);

                                            prefab = Instantiate(oc.prefab, pos, rot);
                                            break;

                                        //create object at mouse position and add to mouse tracking list for rest of action
                                        //do not instantiate anything on the server unless a valid point is given from the client up front
                                        case ObjectCreationLocation.atMouse:
                                            Debug.LogError($"GameAction: {functionCaller.name} using action {data.name} cannot create mouse object at end of action");
                                            break;

                                        case ObjectCreationLocation.atObject:
                                            Transform spawn = functionCaller.transform;
                                            foreach (Transform child in spawn)
                                            {
                                                if (child.tag == "Spawnpoint")
                                                {
                                                    spawn = child;
                                                    break;
                                                }
                                            }

                                            prefab = Instantiate(oc.prefab, spawn.position, spawn.rotation);
                                            break;
                                    }
                                    if (prefab != null)
                                    {
                                        objectsCreated.Add(prefab);
                                        if (oc.destroyAfterAction)
                                            objectsToBeDestroyed.Add(prefab);
                                        if (oc.serverSpawnAfterAction)
                                            objectsToBeSpawned.Add(prefab);
                                        if (!data.clientSide)
                                        {
                                            //objects created serverside need to be created on all clients
                                            NetworkServer.Spawn(prefab);
                                            InitialiseNewPlayerObject(prefab, owningPlayer);
                                        }
                                        else
                                        {
                                            //add any clientside only objects to the preview layer
                                            prefab.layer = LayerMask.NameToLayer("Preview");
                                            foreach (Transform trans in prefab.GetComponentsInChildren<Transform>())
                                            {
                                                trans.gameObject.layer = LayerMask.NameToLayer("Preview");
                                            }
                                        }
                                    }
                                }

                                if (data.clientSide)
                                {
                                    if (objectsToBeSpawned.Count > 0)
                                    {
                                        List<int> indices = new List<int>();
                                        List<Vector3> positions = new List<Vector3>();
                                        List<Quaternion> rotations = new List<Quaternion>();

                                        for (int i = 0; i < objectsToBeSpawned.Count; i++)
                                        {
                                            if (data.objectsToSpawn[i].serverSpawnAfterAction)
                                            {
                                                indices.Add(i);
                                                positions.Add(objectsToBeSpawned[i].transform.position);
                                                rotations.Add(objectsToBeSpawned[i].transform.rotation);
                                            }
                                        }
                                        NetworkActionData networkData = DataToNetwork(data, playerObject);
                                        CmdSpawnObjects(networkData, functionCaller, owningPlayer, indices, positions, rotations);
                                    }

                                    if (data.incomeChange.Count > 0)
                                        CmdIncomeChange(owningPlayer, data.incomeChange);
                                }
                                else
                                {
                                    if (data.incomeChange.Count > 0)
                                        ResourceManager.instance.IncomeChange(playerObject.owningPlayer, owningPlayer, data.incomeChange);
                                }
                            }
                            else
                            {
                                //action failed, destroy everything created by it
                                foreach (GameObject go in objectsCreated)
                                    Destroy(go);
                                objectsToBeDestroyed.Clear();

                                foreach (GameActionData action in data.nextActionsOnFailure)
                                    StartAction(action, playerObject, owningPlayer);
                                loop = false;
                            }

                            //an exit condition has been reached, end this action
                            if (data.queueAction)
                                playerObject.QueuedActionCompleted();
                            break;
                        }
                        else if (data.successConditions.Count > 0)
                        {
                            if (success)
                                EvaluateSuccess(data, playerObject, objectsCreated[0]);
                            else
                            {
                                ConditionEventData conditionEventData = new ConditionEventData();
                                conditionEventData.success = false;
                                conditionEventData.firstSpawnedObject = objectsCreated[0];
                                data.onConditionEvaluate.Invoke(conditionEventData);
                            }
                        }
                    }
                    if (firstTime)
                        firstTime = false;
                    yield return null;
                }

                //cleanup
                data.onActionEnd?.Invoke(playerObject, data);

                foreach (GameObject go in objectsToBeDestroyed)
                    Destroy(go);

                if (data.lockCamera)
                    CameraController.instance.ToggleCameraInputs(previousCameraState);
                if (data.lockInput)
                {
                    PlayerInput.instance.ToggleSingleSelectionInputs(previousSingleSelectionState);
                    PlayerInput.instance.ToggleDragSelectionInputs(previousDragSelectionState);
                    PlayerInput.instance.ToggleMovementInputs(previousMovementState);
                }

            } while (loop);
        }
    }
}
