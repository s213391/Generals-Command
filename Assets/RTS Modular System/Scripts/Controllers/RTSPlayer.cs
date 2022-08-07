using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using DS_FogOfWar;
using DS_Resources;
using DS_BasicCombat;
using DS_Selection;

namespace RTSModularSystem
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GameAction))]
    [RequireComponent(typeof(PlayerObject))]
    public class RTSPlayer : NetworkBehaviour
    {
        public static RTSPlayer localPlayer { get; private set; }
        public static NetworkIdentity identity { get; private set; }
        public static CameraController camController { get; private set; }
        public static SelectionController selectionController { get; private set; }
        public static PlayerInput playerInput { get; private set; }
        public static UnitArrangement unitArrangement { get; private set; }
        public static PlayerObject playerObject { get; private set; }
        public static FogSampler fogSampler { get; private set; }
        public static CombatManager combatManager { get; private set; }

        static Camera mainCam;
        static GameAction gameAction;

        //set up static reference to the local player, but keep non-local players for commands/clientRPCs 
        public void Start()
        {
            //only initialise if this is the local player, else disable
            if (isLocalPlayer)
            {
                if (localPlayer == null)
                    Init();
            }
            else
                enabled = false;
        }


        //initialise or get reference to all required components
        private void Init()
        {
            localPlayer = this;

            //move player to starting position based on host status
            if (isServer == isLocalPlayer)
            {
                transform.position = ObjectDataManager.HostPosition();
                transform.eulerAngles = ObjectDataManager.HostRotation();
            }
            else
            {
                transform.position = ObjectDataManager.ClientPosition();
                transform.eulerAngles = ObjectDataManager.ClientRotation();
            }

            identity = GetComponent<NetworkIdentity>();
            gameAction = GetComponent<GameAction>();
            playerInput = FindObjectOfType<PlayerInput>();
            unitArrangement = FindObjectOfType<UnitArrangement>();
            selectionController = FindObjectOfType<SelectionController>();
            fogSampler = FindObjectOfType<FogSampler>();
            playerObject = GetComponent<PlayerObject>();
            combatManager = GetComponent<CombatManager>();

            mainCam = Camera.main;
            camController = mainCam.GetComponent<CameraController>();

            camController.Init(isServer == isLocalPlayer);
            playerInput.Init();
            combatManager.Init();
            ObjectDataManager.instance.Init();
            CmdCreateResourceDictionaries(GetID());
            CmdBuildMangerInit(playerObject.gameObject, GetID());

            if (GUIController.instance == null)
                FindObjectOfType<GUIController>().Init();
            GUIController.PlayerConnect();
        }


        //ensure everything updates in order, and only once ready
        private void Update()
        {
            if (!isLocalPlayer)
                return;

            camController.OnUpdate();
            playerInput.OnUpdate();
            GUIController.OnUpdate();
        }


        [Command]
        //initialise buildmanager
        private void CmdBuildMangerInit(GameObject go, uint owningPlayer)
        {
            PlayerObject po = go.GetComponent<PlayerObject>();
            if (po != null)
                po.SetID(owningPlayer);
        }


        //passes call to start action to attached game action script
        public static void StartAction(GameActionData data, PlayerObject po)
        {
            gameAction.StartAction(data, po, GetID());
        }


        [Command]
        //same as above, just run on the server
        public void CmdStartAction(NetworkActionData networkData, GameObject functionCaller, NetworkInputData inputData, uint owningPlayer)
        {
            gameAction.StartServerAction(networkData, functionCaller, inputData, owningPlayer);
        }


        //returns the networkID of the player
        public static uint GetID()
        {
            return identity.netId;
        }


        //deactivate GUI on leaving server
        private void OnDestroy()
        {
            if (!isLocalPlayer)
                return;

            GUIController.PlayerDisconnect();
            if (mainCam != null)
                mainCam.GetComponent<CameraController>().canMoveTarget = false;
        }


        //call the resource manager to create a dictionary for this player
        [Command]
        public void CmdCreateResourceDictionaries(uint owningPlayer)
        { 
            ResourceManager.instance.CreateResourceDictionaries(owningPlayer);
        }


        //gets the resource counts of the requested player, defaults to own resources
        public static List<ResourceQuantity> GetResourcesCount(uint requestedID = 0)
        {
            if (requestedID == 0)
                requestedID = GetID();

            return ResourceManager.instance.GetResourceValues(requestedID, GetID());
        }


        //checks if the player has enough resources to pay the given cost
        public static bool CanAffordCost(List<ResourceQuantity> cost, uint requestedID, uint requesterID = 0)
        {
            if (requesterID == 0)
                requesterID = GetID();

            return ResourceManager.instance.IsResourceChangeValid(requestedID, requesterID, cost, true);
        }


        //returns whether this object belongs to the local player
        public static bool Owns(PlayerObject po)
        {
            return po.owningPlayer == GetID();
        }


        [Command]
        //updates the destination of a unit serverside
        public void CmdMoveUnit(GameObject unit, Vector3 destination)
        {
            unitArrangement.MoveUnit(unit, destination);
        }
    }
}
