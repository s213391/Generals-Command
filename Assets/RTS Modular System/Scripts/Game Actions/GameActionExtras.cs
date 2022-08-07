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
        [Tooltip("Whether this object will be destroyed when this action ends. \nUseful for clientside visual effects such as building placement")]
        public bool destroyAfterAction;
    }


    //used for objects following the mouse using different layermasks 
    public struct MouseTrackingObject
    {
        public GameObject obj;
        public LayerMask layerMask;
        public bool snapping;
        public float snapDistance;
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
}