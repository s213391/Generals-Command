using UnityEngine;
using UnityEngine.AI;
using Mirror;
using DS_BasicCombat;
using DS_Selection;

namespace RTSModularSystem
{
    [RequireComponent(typeof(NetworkTransform))]
    public class PlayerObject : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnIDChange))] [SerializeField] [Tooltip("The Network Identity of the LocalPlayer that owns this object")]
        public uint owningPlayer = 9999;
        [SerializeField] [Tooltip("The PlayerObjectData that dictates how this object behaves")]
        public PlayerObjectData data;

        public int currentXP { get; private set; }
        public bool isHidden { get; private set; }

        private NavMeshAgent agent;
        private Attackable attackable;
        private Attacker attacker;
        private Selectable selectable;

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
            currentXP = 0;
            gameObject.SetActive(true);

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

            //REFACTOR Moveable
            //set up movement
            if (data.moveable)
            {
                agent = gameObject.GetComponent<NavMeshAgent>();
                if (agent == null)
                    agent = gameObject.AddComponent<NavMeshAgent>();

                agent.speed = data.moveSpeed;
                agent.height = data.agentHeight;
                agent.radius = data.agentWidth;
                agent.avoidancePriority = data.pathfingPriority;
                agent.autoTraverseOffMeshLink = false;

                if (data.passThroughOtherAgents)
                    agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
                else
                    agent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;
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
                float height = 0.0f;
                if (data.moveable)
                    height = agent.height;
                else
                    height = GetComponent<NavMeshObstacle>().height;

                attackable = gameObject.AddComponent<Attackable>();
                attackable.Init(height, data.maxHealth, data.resistances, data.xpOnDeath);
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
            Color colour;
            LayerMask mask;
            if (ownedByLocalPlayer)
            {
                colour = Color.blue;
                mask = LayerMask.NameToLayer("Friendly");
            }
            else
            {
                colour = Color.red;
                mask = LayerMask.NameToLayer("Enemy");
            }

            foreach (Transform trans in transforms)
            {
                if (trans.gameObject.layer == 0)
                    trans.gameObject.layer = mask;

                Renderer renderer = trans.GetComponent<Renderer>();
                if (renderer == null)
                    continue;

                //set the Replaceable material to team colour
                Material material = renderer.material;
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    if (renderer.materials[i].name == "Replaceable (Instance)")
                    {
                        renderer.materials[i].color = colour;
                        break;
                    }
                }

                //hide mobile enemy units
                if (!ownedByLocalPlayer && data.moveable)
                {
                    isHidden = true;
                    renderer.enabled = false;
                }
            }

            //start any actions marked as auto start
            if (ownedByLocalPlayer)
                foreach (StartingAction sa in data.actions)
                    if (sa.autoStart)
                        RTSPlayer.StartAction(sa.action, this);
        }


        //update health and visibility
        private void Update()
        {
            //calls init once the owning player ID has set
            if (!initialised && owningPlayer != 9999)
                Init();

            //update components
            attackable?.OnUpdate();

            //only update visibilty on initialised moveable enemy objects
            if (owningPlayer == 9999 || !data.moveable || (RTSPlayer.localPlayer && RTSPlayer.Owns(this)))
                return;

            bool visible = RTSPlayer.fogSampler.IsVisible(transform.position);

            if (visible != isHidden)
                return;

            isHidden = !visible;

            //update the state of the renderers
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
                renderer.enabled = visible;
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


        //REFACTOR Split Attackable
        //stop rendering object, and prevent further interaction
        private void ZeroHealth()
        {
            //if object does not persist, destroy completely, else destroy as much as possible
            if (!data.persistAtZeroHealth)
            {
                interrupt = 9;
                Destroy(gameObject);
                return;
            }

            interrupt = 8;

            //destroy children
            for (int i = 0; i < transform.childCount; i++)
                Destroy(transform.GetChild(i).gameObject);

            //destroy renderer, collider and navmesh components
            Renderer[] renderers = GetComponents<Renderer>();
            foreach (Renderer renderer in renderers)
                Destroy(renderer);
            Collider[] colliders = GetComponents<Collider>();
            foreach (Collider collider in colliders)
                Destroy(collider);
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            if (agent != null)
                Destroy(agent);
            NavMeshObstacle obstacle = GetComponent<NavMeshObstacle>();
            if (obstacle != null)
                Destroy (obstacle);

            Destroy(attackable);
        }


        //performs the action at the given index
        public void StartAction(int index)
        {
            RTSPlayer.StartAction(data.actions[index].action, this);
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


        //REFACTOR Moveable
        //moves the object to the given position
        public void MoveTo(Vector3 pos)
        {
            agent.SetDestination(pos);
        }
    }
}
