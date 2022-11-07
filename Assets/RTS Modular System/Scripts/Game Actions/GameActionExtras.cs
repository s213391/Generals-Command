using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RTSModularSystem
{
    //user inputs or other triggers that can end actions
    public enum ActionEndType
    {
        leftClick,
        rightClick,
        escape,
        enter,
        immediate,
        duration,
        trigger,
        none //does not end
    }


    //conditions for ending an action
    [System.Serializable]
    public struct ActionEnd
    {
        public bool successfulEnd; //whether this condition will start the next action or simply end the action
        public ActionEndType type;
        [ConditionalHide("type", "duration")]
        public float seconds;
    }


    //conditions that decide whether an action is successful or not
    public enum ActionConditionType
    { 
        proximityToObjects,
        collidingWithLayers,
        onTerrain
    }


    public enum ObjectToBeChecked
    { 
        self,
        objectSpawnedByThisAction
    }


    [System.Serializable]
    public struct ActionCondition
    {
        [HideInInspector]
        public bool conditionMet; //whether this condition is currently being met
        public bool successIfConditionMet; //whether meeting this condition is considered a success or failure
        public ActionConditionType type;
        public ObjectToBeChecked objectToBeChecked;
        [ConditionalHide("type", "proximityToObjects")]
        public float distance;
        [ConditionalHide("type", "proximityToObjects")]
        public PlayerObjectData objectsType;
        [ConditionalHide("type", "proximityToObjects")]
        public DS_BasicCombat.TargetType teamsToCheck;
        [ConditionalHide("type", "collidingWithLayers")]
        public LayerMask layers;
        [ConditionalHide("type", "onTerrain")]
        public float maximumHeightAllowed;
    }


    public struct ConditionEventData
    {
        public GameObject functionCaller;
        public GameObject firstSpawnedObject;
        public List<ActionCondition> conditions;
        public bool success;
    }


    //options for how to set the spawn location for a prefab during an action
    public enum ObjectCreationLocation
    {
        atPoint, //uses pos/rot data stored in the prefab
        atMouse, //uses the mouse position
        atObject, //uses the transform of a given object
    }


    //a spawnable prefab and how/where to spawn it
    [System.Serializable]
    public struct ObjectCreation
    {
        [Tooltip("The object prefab that will be spawned")]
        public GameObject prefab;
        [Tooltip("How the spawn location is chosen. \natPoint - spawns at a given local or world position. \natMouse - spawns wherever the mouse is on the screen. \natObject - spawns at the object that has this action, or a child it has called \"Spawnpoint\"")]
        public ObjectCreationLocation location;
        [ConditionalHide("location", "atPoint")] [Tooltip("Whether this position is world space or local space")]
        public bool worldPosition;
        [ConditionalHide("location", "atPoint")] [Tooltip("The world space position this object will spawn at")]
        public Vector3 position;
        [ConditionalHide("location", "atPoint")] [Tooltip("Whether this rotation is world space or local space")]
        public bool worldRotation;
        [ConditionalHide("location", "atPoint")] [Tooltip("The world space rotation this object will spawn at")]
        public Vector3 rotation;
        [ConditionalHide("location", "atMouse")] [Tooltip("The layers that this object will appear on when moused over")]
        public LayerMask mouseLayerMask;
        [ConditionalHide("location", "atMouse")] [Tooltip("If this object will snap itself to the transform of a nearby object that has a given PlayerObjectData")]
        public bool snapToObject;
        [ConditionalHide("snapToObject", "true")] [Tooltip("The distance at which this object will check for nearby objects to snap to")]
        public float snapDistance;
        [ConditionalHide("snapToObject", "true"), Tooltip("Only move the spawned object when the cursor is over it, else fix the object relative to the camera")]
        public bool onlyMoveWhenUnderCursor;
        [Tooltip("Whether this object will be created on the client at the start of the action, or the end of it")]
        public bool createAfterAction;
        [Tooltip("Whether this object will be destroyed when this action ends. \nUseful for clientside visual effects such as building placement")]
        public bool destroyAfterAction;
        [Tooltip("Only useful if run on a clientside action. Whether this object will be spawned on the server at its current position when this action ends successfully.")]
        public bool serverSpawnAfterAction;
    }


    //used for objects following the mouse using different layermasks 
    public struct MouseTrackingObject
    {
        public GameObject obj;
        public LayerMask layerMask;
        public bool snapping;
        public float snapDistance;
        public bool onlyMoveWhenUnderCursor;
    }


    //used for communicating actions across the network
    public struct NetworkActionData
    {
        public uint owningPlayer;
        public PlayerObjectType objectType;
        public int objectIndex;
        public int actionIndex;
    }


    //used to communicate client input to the server
    public struct NetworkInputData
    {
        public Ray mouseRay;
        //public int[] mouseButtonsUp;
        //public int[] mouseButtons;
        //public KeyCode[] keysDown;
        //public KeyCode[] keys;
    }


    [System.Serializable]
    //used in player objects to tell which actions start when the object spawns
    public struct StartingAction
    {
        public GameActionData action;
        public bool autoStart;
    }


    //holds reference to a trigger that can be used to end an action
    public struct ActionTrigger
    {
        public PlayerObject objectPerformingAction;
        public GameActionData actionBeingPerformed;
        public bool triggered;
    }
}