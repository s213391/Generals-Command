using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DS_BasicCombat;

namespace RTSModularSystem
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Player Object Data", order = 101)]
    [System.Serializable]
    public class PlayerObjectData : ScriptableObject
    {
        [Tooltip("Used to set up a designer-defined categorisation.\nThis data will also need to be placed in the matching array in the Object Data Manager")]
        public PlayerObjectType objectType;

        [Header("Combat - Attackable")]
        [Tooltip("Whether this object can be attacked by other objects")]
        public bool attackable = false;
        [ConditionalHide("attackable", "true")] [Min(1)] [Tooltip("The amount of damage required to destroy this object")]
        public int maxHealth = 1;
        /*[ConditionalHide("attackable", "true")] Array ignores attribute*/[Tooltip("The percentage of damage that is removed before it affects health, for the given damage types")]
        public List<DamageResistance> resistances;
        [ConditionalHide("attackable", "true"), Tooltip("The height above the ground that this attackable's health bar appears")]
        public float healthBarHeight = 1.0f;
        [ConditionalHide("attackable", "true"), Tooltip("The pixel width of this attackable's health bar")]
        public int healthBarWidth = 100;
        [ConditionalHide("attackable", "true")] [Tooltip("The amount of xp given to whatever kills this unit. \nCan be negative to disincentivise killing this object")]
        public int xpOnDeath = 0;
        [ConditionalHide("attackable", "true")] [Tooltip("Whether unity destroys this object when it reaches 0 health. \nAll children gameobjects, navmesh components, renderers and colliders will still be destroyed regardless. \nSet to true if this object has an action that you want to continue performing even after it is killed")]
        public bool persistAtZeroHealth = false;
        [ConditionalHide("attackable", "true")]
        public UnityEvent onDamage;
        [ConditionalHide("attackable", "true")]
        public UnityEvent onHeal;
        [ConditionalHide("attackable", "true")]
        public UnityEvent onDeath;

        [Header("Combat - Attacker")]
        [Tooltip("Whether this object can attack other objects")]
        public bool attacker = false;
        [ConditionalHide("attacker", "true")] [Tooltip("What kind of attack this object does")]
        public AttackType attackType = AttackType.melee;
        [ConditionalHide("attacker", "true")] [Tooltip("What type of damage this attck does")]
        public DamageType damageType = DamageType.light;
        [ConditionalHide("attacker", "true")] [Tooltip("Which units this attacker targets")]
        public TargetType targetType = TargetType.enemy;
        [ConditionalHide("attacker", "true")] [Tooltip("How much damage this object does in a single attack")]
        public int attackDamage = 5;
        [ConditionalHide("attacker", "true")] [Tooltip("The maximum distance away this object can hit an enemy. \nStill useful to set a range for melee to hit objects that are moving or running away")]
        public float attackRange = 0.5f;
        [ConditionalHide("attacker", "true")] [Tooltip("How long an attack cycle takes to complete before the attack can begin again")]
        public float attackDuration = 1.0f;
        [ConditionalHide("attacker", "true")] [Tooltip("If this object can automatically target enemy objects")]
        public bool canAutoTarget = false;
        [ConditionalHide("canAutoTarget", "true")] [Tooltip("The range that this unit will automatically target any attackable enemy object. \nThis object will only check for targets if it is neither moving, nor targeting another object")]
        public float autoTargetRange = 1.0f;
        [ConditionalHide("canAutoTarget", "true")]
        public UnityEvent onAttack;

        [Header("Leveling")]
        [Tooltip("Whether this object can level up or improve. \nIf this data is the maximum level, set it to false")]
        public bool levelable = false;
        [ConditionalHide("levelable", "true")] [Tooltip("The level of this object data")]
        public int level = 1;
        [ConditionalHide("levelable", "true")] [Tooltip("When this object levels up this data will replace it")]
        public PlayerObjectData nextLevel = null;
        [ConditionalHide("levelable", "true")] [Tooltip("When this object has accumlated this much xp over its lifetime, it will move to the next level. \n-1 to ignore xp and instead level up using a trigger")]
        public int xpRequirement = -1;

        [Header("Movement")]
        [Tooltip("Whether this object can move")]
        public bool moveable = false;
        [ConditionalHide("moveable", "true")] [Tooltip("The maximum speed this object can move")]
        public float moveSpeed = 1.0f;
        [ConditionalHide("moveable", "true")] [Tooltip("The height the navmesh agent will be set to for obstacle avoidance")]
        public float agentHeight = 2.0f;
        [ConditionalHide("moveable", "true")] [Tooltip("The radius width the navmesh agent will be set to for obstacle avoidance")]
        public float agentWidth = 0.5f;
        [ConditionalHide("moveable", "true")] [Range (0, 99)]  [Tooltip("0 is highest priority, 99 is lowest priority. \nHigher priority objects will ignore lower priority object when pathing and will push them out of the way if any lower priority agent is in its path")]
        public int pathfingPriority = 50;
        [ConditionalHide("moveable", "true")] [Tooltip("Whether this object will walk through other moving objects or push around them. \nIf true, navmesh obstacles will need to be set to carve, or this object will not path around them")]
        public bool passThroughOtherAgents = true;
        [ConditionalHide("moveable", "true")]
        public UnityEvent onMoveBegin;
        [ConditionalHide("moveable", "true")]
        public UnityEvent onMoveEnd;

        [Header("Required Settings")]
        [Tooltip("A brief summary of what this object is and what it does")]
        public string description;
        [Tooltip("The maximum amount of this object allowed to exist at once, per team. -1 for infinite")]
        public int unitCap = -1; 
        [Tooltip("The range in metres that this unit can see around itself at different heights. \nEach vector is read as (horizontal range, vertical offset from ground) \nUsed to allow different vision over areas of the map that are lower or higher than this object")]
        public List<Vector2> visibilityRanges;
        [Tooltip("The image that will be used in the GUI when this object is selected")]
        public Sprite sprite;
        [Tooltip("If this object can be selected")]
        public bool selectable;

        [Space]
        [Tooltip("The actions this object can perform, and whether they will be started automatically")]
        public List<StartingAction> actions;
    }
}