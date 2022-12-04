using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace RTSModularSystem.BasicCombat
{
    //Class that handles range checking for all Attacker objects
    public class CombatManager : NetworkBehaviour
    {
        public static CombatManager instance { get; private set; }

        public float secondsBetweenTargetChecks = 1.0f;
        public LayerMask friendlyObjectLayers;
        public LayerMask enemyObjectLayers;

        public float downtimeBeforeCombatOver;
        float timeSinceCombat = 0.0f;
        public static bool inCombat { get; private set; }

        //private HashSet<Attacker> attackers;
        //private HashSet<Attackable> attackables;

        public List<Attacker> attackers = new List<Attacker>();
        public List<Attackable> attackables = new List<Attackable>();

        private List<Attackable> attackablesToBeDestroyed = new List<Attackable>();

        struct AttackableDistance
        {
            public Attackable target;
            public float range;
        }

        //set up singleton and create all lists automatically
        void Start()
        {
            Init();
        }


        //set up singleton and create all lists manually
        public void Init()
        {
            if (instance == null && isLocalPlayer)
            {
                instance = this;

                //attackers = new HashSet<Attacker>();
                //attackables = new HashSet<Attackable>();

                timeSinceCombat = downtimeBeforeCombatOver;
                inCombat = false;
            }
        }


        //check all attackers against all opposing attackables and set targets on the server
        void Update()
        {
            if (instance != this)
                return;

            timeSinceCombat += Time.deltaTime;

            if (timeSinceCombat < downtimeBeforeCombatOver)
                inCombat = true;
            else
                inCombat = false;


            //only check combat on server player object
            if (!isServer || !isLocalPlayer)
                return;
            
            foreach (Attacker attacker in attackers)
            {
                //check if this attacker needs a new target and start the targetting cooldown
                if (attacker.NeedsTarget(true))
                {
                    //set variables now so they persist after the foreach loop
                    Vector3 attackerPosition = attacker.transform.position;

                    //check if attacker is on friendly layer
                    bool attackerIsFriendly = (friendlyObjectLayers & (1 << attacker.gameObject.layer)) != 0;

                    //set which objects to check relative to attacker layer
                    LayerMask targetLayers;
                    if (attacker.targetType == TargetType.enemy)
                    {
                        if (attackerIsFriendly)
                            targetLayers = enemyObjectLayers;
                        else
                            targetLayers = friendlyObjectLayers;
                    }
                    else if (attacker.targetType == TargetType.friendly)
                    {
                        if (attackerIsFriendly)
                            targetLayers = friendlyObjectLayers;
                        else
                            targetLayers = enemyObjectLayers;
                    }
                    else
                        targetLayers = enemyObjectLayers | friendlyObjectLayers;


                    //use a sphere collision check to see if any attackables are in range
                    Collider[] objectsInRange = Physics.OverlapSphere(attackerPosition, attacker.TargetRange(), targetLayers);
                    List<AttackableDistance> potentialTargets = new List<AttackableDistance>();

                    foreach (Collider collider in objectsInRange)
                    {
                        if (collider.TryGetComponent<Attackable>(out Attackable attackable))
                        {
                            float distance = Vector3.Distance(collider.ClosestPoint(attackerPosition), attackerPosition);

                            AttackableDistance potentialTarget = new AttackableDistance() { target = attackable, range = distance };
                            potentialTargets.Add(potentialTarget);
                        }
                    }

                    if (potentialTargets.Count > 0)
                    {
                        potentialTargets.Sort((x, y) => x.range.CompareTo(y.range));

                        attacker.TrySetTarget(potentialTargets[0].target, potentialTargets[0].range, false);

                        if (attacker.attackType == AttackType.rangedArc)
                            StartCoroutine(ArcedAttack(attacker, potentialTargets[0].target.transform.position, targetLayers));
                    }
                }

                //try to attack target
                attacker.TryAttack();
            }

            foreach (Attackable attackable in attackablesToBeDestroyed)
                RemoveCombatObject(attackable.gameObject);
            attackablesToBeDestroyed.Clear();
        }


        //sends attack to the server to handle
        public void Attack(Attackable attackable, DamageType type, int damage, Attacker attacker)
        {
            CmdAttack(attackable.gameObject, type, damage, attacker.gameObject);
        }


        [Command]
        //handles attack on server
        private void CmdAttack(GameObject attackableGO, DamageType type, int damage, GameObject attackerGO)
        {
            Attackable attackable = attackableGO.GetComponent<Attackable>();
            Attacker attacker = attackerGO.GetComponent<Attacker>();

            attackable.HealthChange(type, damage, attacker);
            RpcSetHealthClient(attackableGO, attackable.currentHealth);
        }


        [ClientRpc]
        //updates health value on the clients
        public void RpcSetHealthClient(GameObject attackable, int newHealth)
        {
            attackable?.GetComponent<Attackable>()?.SetHealth(newHealth);
        }


        //adds a gameobject with an attackable and/or attacker to the correct arrays
        public void AddCombatObject(GameObject go)
        {
            CmdAddCombatObject(go);
        }


        //removes a gameobject with an attackable and/or attacker from the correct arrays
        public void RemoveCombatObject(GameObject go)
        {
            CmdRemoveCombatObject(go);
        }


        [Command]
        //adds an attackable and/or attacker to the correct arrays
        private void CmdAddCombatObject(GameObject go)
        {
            if (go.TryGetComponent<Attacker>(out Attacker attacker))
                attackers.Add(attacker);
            if (go.TryGetComponent<Attackable>(out Attackable attackable))
                attackables.Add(attackable);
        }


        [Command]
        //removes an attackable and/or attacker from the correct arrays
        private void CmdRemoveCombatObject(GameObject go)
        {
            if (go.TryGetComponent<Attacker>(out Attacker attacker))
                attackers.Remove(attacker);
            if (go.TryGetComponent<Attackable>(out Attackable attackable))
                attackables.Remove(attackable);
        }


        //sets an attackable to be destroyed after every attacker has attacked
        public void MarkForDestruction(Attackable attackable)
        {
            if (!attackablesToBeDestroyed.Contains(attackable))
                attackablesToBeDestroyed.Add(attackable);
        }


        //tells the combat manager to consider this player in combat
        public void CombatOccured()
        {
            timeSinceCombat = 0.0f;
            inCombat = true;
        }


        //triggers an AOE attack after a set amount of time
        IEnumerator ArcedAttack(Attacker attacker, Vector3 attackPosition, LayerMask targetLayers)
        {
            yield return new WaitForSeconds(1.5f);
            
            Collider[] objectsInRange = Physics.OverlapSphere(attackPosition, 25.0f, targetLayers);
            attacker.attackerEvents?.OnAttack();

            foreach (Collider collider in objectsInRange)
                if (collider.TryGetComponent(out Attackable attackable))
                    Attack(attackable, DamageType.artillery, attacker.attackDamage, attacker);
        }
    }
}
