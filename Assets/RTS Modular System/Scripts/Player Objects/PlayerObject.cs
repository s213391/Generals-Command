using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Mirror;
using RTSModularSystem.BasicCombat;
using RTSModularSystem.Selection;
using RTSModularSystem.GameResources;

namespace RTSModularSystem
{
    [RequireComponent(typeof(NetworkTransform))]
    public class PlayerObject : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnIDChange))] [SerializeField] [Tooltip("The Player Number of the LocalPlayer that owns this object")]
        public uint owningPlayer = 9999;
        [SerializeField] [Tooltip("The PlayerObjectData that dictates how this object behaves")]
        public PlayerObjectData data;

        public bool isHidden { get; private set; }

        private Attackable attackable;
        private Attacker attacker;
        private Selectable selectable;
        private Moveable moveable;

        public List<GameActionData> queuedActions;

        private bool initialised = false;

        //all actions that this player object has started will end this frame if their interruption tolerance is equal to or below this value
        //value will reset itself to zero in late update
        public int interrupt { get; private set; }
        
        
        [Server]
        //set owning player ID
        public void SetID(uint ID)
        {
            OnIDChange(owningPlayer, ID);
        }


        //IDs are only changed when the server spawns player objects
        public void OnIDChange(uint oldID, uint newID)
        {
            //do not update ID until local player has been initialised
            if (RTSPlayer.localPlayer == null)
                return;

            //protection against ownership changing after initialisation
            if (oldID != 9999)
                return;

            if (owningPlayer != newID)
                owningPlayer = newID;
        }


        //initialise all components
        public void Init()
        { 
            initialised = true;
            bool ownedByLocalPlayer = RTSPlayer.Owns(this);

            //set up basic properties
            interrupt = 0;
            gameObject.SetActive(true);
            queuedActions = new List<GameActionData>();

            //set up selection
            if (data.selectable)
            {
                selectable = gameObject.AddComponent<Selectable>();
                selectable.Init();
                selectable.UIIcon = data.sprite;
            }

            //REFACTOR Vision
            //set up vision for owned units
            if (ownedByLocalPlayer && data.visibilityRanges.Count > 0)
            {
                //for every vector2 in the visibiltyRanges list create a disc that will only be seen by the fog of war cameras
                for (int i = 0; i < data.visibilityRanges.Count; i++)
                {
                    Vector2 visibility = data.visibilityRanges[i];
                    GameObject vision = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    vision.transform.parent = transform;
                    vision.transform.localPosition = new Vector3(0.0f, visibility.y, 0.0f);
                    vision.transform.localScale = new Vector3(visibility.x, 0.1f, visibility.x);
                    vision.layer = LayerMask.NameToLayer("Vision");
                    Destroy(vision.GetComponent<SphereCollider>());
                    vision.GetComponent<Renderer>().material.renderQueue = 2002;
                }
            }

            //set up movement
            if (data.moveable)
            {
                moveable = gameObject.AddComponent<Moveable>();
                moveable.Init(data.moveSpeed, data.acceleration, data.angularSpeed, data.agentHeight, data.agentWidth, data.pathfingPriority, data.passThroughOtherAgents);
            }
            //REFACTOR Obstacle
            else
            {
                //if not movable and has a collider, turn into obstacle
                Collider col = gameObject.GetComponent<Collider>();
                if (col != null)
                {
                    NavMeshObstacle nmo = gameObject.AddComponent<NavMeshObstacle>();
                    if (col is CapsuleCollider)
                    {
                        CapsuleCollider capCol = col as CapsuleCollider;
                        nmo.shape = NavMeshObstacleShape.Capsule;
                        nmo.radius = capCol.radius;
                        nmo.height = capCol.height;
                        nmo.carving = true;
                    }
                }
            }

            //create a health bar for attackable objects
            //attackable player objects must have a collider
            if (data.attackable && GetComponent<Collider>() != null)
            {
                attackable = gameObject.AddComponent<Attackable>();
                attackable.Init(data.healthBarHeight, data.healthBarWidth, data.maxHealth, data.resistances, data.xpOnDeath);
            }

            if (data.attacker)
            {
                attacker = gameObject.AddComponent<Attacker>();
                attacker.Init(data.attackType, data.damageType, data.targetType, data.attackDamage, data.attackRange, data.attackDuration, data.xpRequirement != -1, data.canAutoTarget, data.autoTargetRange);
            }

            //add gameobject to combat manager
            if (data.attackable || data.attacker)
                CombatManager.instance.AddCombatObject(gameObject);

            //set up layers and colours
            Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
            Color colour = GameData.instance.playerData[(int)owningPlayer - 1].colour;
            LayerMask mask;
            if (ownedByLocalPlayer)
                mask = LayerMask.NameToLayer("Friendly");
            else
                mask = LayerMask.NameToLayer("Enemy");

            foreach (Transform trans in transforms)
            {
                if (trans.gameObject.layer == 0)
                    trans.gameObject.layer = mask;

                Renderer renderer = trans.GetComponent<Renderer>();
                if (renderer == null)
                    continue;

                //set the material to show team colour
                Material material = renderer.material;
                material.SetColor("_TeamColour", colour);
            }

            //add to object data manager for quick reference
            ObjectDataManager.AddPlayerObject(owningPlayer, this);

            //add to minimap
            Minimap.instance.RegisterIcon(this, data.sprite, colour);

            //start any actions marked as auto start
            if (ownedByLocalPlayer)
                foreach (StartingAction sa in data.actions)
                    if (sa.autoStart)
                        RTSPlayer.StartAction(sa.action, this);

            if (TryGetComponent(out PlayerObjectEvents events))
                events.OnSpawn(ownedByLocalPlayer);
        }


        //update health and visibility
        private void Update()
        {
            //calls init once the owning player ID has set
            if (!initialised)
            {
                if (RTSPlayer.localPlayer && owningPlayer != 9999)
                    Init();
                else
                    return;
            }

            //update components
            attackable?.OnUpdate();
            moveable?.OnUpdate();
        }


        //used to reset interrupt value after all action coroutines called
        private void LateUpdate()
        {
            if (interrupt != 0)
                interrupt = 0;
        }


        //increase the interrupt value to end actions early
        public void SetInterrupt(int value)
        {
            if (value > interrupt)
                interrupt = value;
        }


        //remove object references from other scripts and destroy
        public void DestroyPlayerObject()
        {
            ObjectDataManager.RemovePlayerObject(owningPlayer, this);
            if (attackable)
                Destroy(attackable);
            if (attacker)
                Destroy(attacker);
            if (moveable)
                Destroy(moveable);
            if (selectable)
                Destroy(selectable);

            interrupt = 9;
            Minimap.instance.UnRegisterIcon(this);
        }


        //either performs the action at the given index immediately or adds it to the queue to be performed in turn
        public void StartAction(int index)
        {
            GameActionData actionData = data.actions[index].action;

            if (actionData.changeResourcesAtStart && actionData.resourceChange.Count > 0)
                RTSPlayer.ApplyCost(CostModifier.GetModifiedCost(actionData), owningPlayer);

            if (actionData.queueAction)
            {
                queuedActions.Add(actionData);
                if (queuedActions.Count > 1)
                    return;
            }

            RTSPlayer.StartAction(actionData, this);
        }


        //tells the player object that the queued action has completed and to start the next one
        public void QueuedActionCompleted()
        {
            queuedActions.RemoveAt(0);

            if (queuedActions.Count > 0)
                RTSPlayer.StartAction(queuedActions[0], this);
        }


        //returns the index of the matching action stored in data
        public int GetActionIndex(GameActionData actionData)
        {
            for (int i = 0; i < data.actions.Count; i++)
                if (data.actions[i].action.name == actionData.name)
                    return i;
            //no match return -1
            return -1;
        }


        //returns the action at the index given
        public GameActionData GetActionData(int index)
        {
            if (data.actions.Count > index)
                return data.actions[index].action;
            //no match return null
            return null;
        }
    }
}
