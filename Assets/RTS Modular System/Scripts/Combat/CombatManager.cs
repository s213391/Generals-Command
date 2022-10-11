using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace DS_BasicCombat
{
    //Class that handles range checking for all Attacker objects
    public class CombatManager : NetworkBehaviour
    {
        public static CombatManager instance { get; private set; }

        public float secondsBetweenTargetChecks = 1.0f;
        public LayerMask friendlyObjectLayers;
        public LayerMask enemyObjectLayers;

        //private HashSet<Attacker> attackers;
        //private HashSet<Attackable> attackables;

        public List<Attacker> attackers = new List<Attacker>();
        public List<Attackable> attackables = new List<Attackable>();

        private List<Attackable> attackablesToBeDestroyed = new List<Attackable>();

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
            }
        }


        //check all attackers against all opposing attackables and set targets on the server
        void Update()
        {
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
                    Attackable closestAttackable = null;
                    float closestDistance = attacker.TargetRange() + 1.0f;

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
                    foreach (Collider collider in objectsInRange)
                    {
                        //if the collider does not have an Attackable, it cannot be targetted
                        if (!collider.TryGetComponent<Attackable>(out Attackable attackable))
                            continue;

                        //check if this Attackable is the closest
                        Vector3 point = collider.ClosestPoint(attackerPosition);
                        float distance = Vector3.Distance(point, attackerPosition);
                        if (distance != 0 && distance < closestDistance)
                        {
                            closestAttackable = attackable;
                            closestDistance = distance;
                        }
                    }

                    //set the closest attackable as the attacker's new target
                    if (closestAttackable != null)
                        attacker.TrySetTarget(closestAttackable, closestDistance, false);
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
            attackable.GetComponent<Attackable>().SetHealth(newHealth);
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
    }
}
