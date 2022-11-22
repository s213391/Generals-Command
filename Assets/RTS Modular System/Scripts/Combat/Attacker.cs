using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using RTSModularSystem;

namespace RTSModularSystem.BasicCombat
{
    //the different types of damage that each have different resistances
    public enum DamageType
    {
        light,
        sustained,
        heavy,
        artillery
    }


    //the types of attack, used to calculate if objects are in range to attack, and which ones to damage
    public enum AttackType
    {
        melee,
        rangedDirect,
        rangedArc,
        areaOfEffect
    }


    //enums for choosing whether this attacker hits friendly units, enemy units, or both
    public enum TargetType
    {
        enemy,
        friendly,
        everyone
    }


    [DisallowMultipleComponent]
    //gives objects the ability to attack objects with an attackable component
    public class Attacker : MonoBehaviour
    {
        [Header("Attack settings")]
        public AttackType attackType = AttackType.melee;
        public DamageType damageType = DamageType.light;
        public TargetType targetType = TargetType.enemy;
        public int attackDamage = 5;
        public float attackRange = 0.5f;
        public float attackDuration = 1.0f;
        public bool canAccumulateXP = false;
        public int xpTotal = 0;

        [Header("Targeting settings")]
        [SerializeField]
        private Attackable target;
        [SerializeField]
        private bool canAutoTarget = true;
        [SerializeField]
        private float autoTargetRange = 2.0f;
        [SerializeField]
        private bool targetWasSetByPlayer = false;

        private float secondsBetweenTargetChecks;
        private bool targetCheckedRecently = false;
        private bool currentlyAttacking = false;

        private float forwardAiming = 1.0f;
        private float rightAiming = 1.0f;

        public AttackerEvents attackerEvents;

        // Start is called before the first frame update
        void Start()
        {

        }


        //set values externally
        public void Init(AttackType aType, DamageType dType, TargetType tType, int damage, float range, float duration, bool xpTrue, bool canTarget, float targetRange = 0.0f)
        {
            attackType = aType;
            damageType = dType;
            targetType = tType;
            attackDamage = damage;
            attackRange = range;
            attackDuration = duration;
            canAccumulateXP = xpTrue;
            xpTotal = 0;
            canAutoTarget = canTarget;
            autoTargetRange = targetRange;
            secondsBetweenTargetChecks = CombatManager.instance.secondsBetweenTargetChecks;

            attackerEvents = GetComponent<AttackerEvents>();
            if (attackerEvents)
                attackerEvents.Init();

            StartCoroutine(TargettingCooldownDuration());
        }


        //checks the angle towards the target and passes it to the animator
        public void OnUpdate()
        {
            if (target)
            {
                forwardAiming = Vector3.Dot(transform.forward, target.transform.position - transform.position);
                rightAiming = Vector3.Dot(transform.right, target.transform.position - transform.position);
            }
            else
            {
                forwardAiming = 1.0f;
                rightAiming = 0.0f;
            }

            attackerEvents?.OnUpdate(forwardAiming, rightAiming);
        }


        //sets the given Attackable as target if it is in range or set by the player
        public void TrySetTarget(Attackable attackable, float distance, bool setByPlayer)
        {
            if (setByPlayer || (canAutoTarget && distance < autoTargetRange))
            {
                target = attackable;
                targetWasSetByPlayer = setByPlayer;
                if (targetWasSetByPlayer)
                    attackerEvents?.OnTarget();
            }
        }


        //attacker will try to attack target if it is in range and not currently attacking
        public void TryAttack()
        {
            if (target == null || currentlyAttacking)
                return;

            //get closest point on the target and use it for range checking
            Vector3 closestPointOnTarget = target.GetComponent<Collider>().ClosestPoint(transform.position);
            float distance = (transform.position - closestPointOnTarget).magnitude;

            //begin attack and start the attack cooldown
            if (distance <= attackRange)
            {
                if (!(attackType == AttackType.rangedArc))
                    CombatManager.instance.Attack(target, damageType, attackDamage, this);

                currentlyAttacking = true;
                StartCoroutine(AttackDuration());

                attackerEvents?.OnAttack();
            }
        }


        //the attacker will make an AOE attack at the given position
        /*public void MakeAOEAttack(Vector3 position, float radius)
        {
            Physics.OverlapSphere(position, radius, )
        }*/


        //updates the xp total
        public void UnitKill(int xp)
        {
            attackerEvents?.OnKill();

            if (canAccumulateXP)
            {
                xpTotal += xp;
                if (xpTotal < 0)
                    xpTotal = 0;
            }
        }


        //check if attacker has a valid target
        public bool HasValidTarget()
        {
            //if no target is set, return false
            if (target == null)
                return false;

            //if target is no longer visible, remove target and return false
            if (target.currentHealth < 0 || !target.isVisible)
            {
                target = null;
                return false;
            }
            //if target was auto set and is not in attack range, remove target and return false
            else if (!targetWasSetByPlayer)
            {
                if ((target.transform.position - transform.position).magnitude > attackRange)
                { 
                    target = null;
                    return false;
                }
                else if (attackType == AttackType.rangedDirect && Physics.Linecast(transform.position + Vector3.up, target.transform.position + Vector3.up, LayerMask.GetMask("Default", "Terrain")))
                {
                    target = null;
                    return false;
                }
            }

            //target is not invalid, return true
            return true;
        }


        //returns whether this attacker needs a new target and starts targetting cooldown
        public bool NeedsTarget(bool startCooldown = false)
        {
            if (!canAutoTarget || targetCheckedRecently || HasValidTarget())
                return false;

            if (startCooldown)
            {
                targetCheckedRecently = true;
            }
            return true;
        }


        //returns targeting range
        public float TargetRange()
        {
            return autoTargetRange;
        }


        //end attack status after duration has elapsed
        private IEnumerator AttackDuration()
        {
            yield return new WaitForSeconds(attackDuration);
            currentlyAttacking = false;
        }


        //allow target checking after duration has elapsed
        private IEnumerator TargettingCooldownDuration()
        {
            while (true)
            {
                yield return new WaitForSeconds(secondsBetweenTargetChecks);
                targetCheckedRecently = false;
            }
        }
    }
}
