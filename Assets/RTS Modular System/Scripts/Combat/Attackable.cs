using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Mirror;

namespace DS_BasicCombat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    //Class that adds functionality for this object to be attacked by objects with an Attacker component and display a health bar
    public class Attackable : MonoBehaviour
    {
        [Tooltip("Will run automatically using Start and Update. \nIf false, will require Init and OnUpdate to be called in script")]
        public bool automaticUsage = false;
        public int currentHealth = 1;
        public int maxHealth = 1;
        public int xpOnDeath = 0;
        public float objectHeight = 1.0f;
        public float objectWidth = 100.0f;
        public bool isVisible = true;

        Resistances resistances;
        HealthBar healthBar;

        public UnityEvent<GameObject, int, int> onDamage;
        public UnityEvent<GameObject, int, int> onHeal;
        public UnityEvent<GameObject> onDeath;


        //set up using inspector values
        private void Start()
        {
            if (!automaticUsage)
            {
                //disable update
                enabled = false;
                return;
            }

            healthBar = HealthBarManager.instance.AddHealthBar(this);
            healthBar.Init(objectHeight, objectWidth);
        }


        //set up values externally
        public void Init(float height, float width, int health, List<DamageResistance> resists, int xp = 0)
        {
            currentHealth = health;
            maxHealth = health;
            xpOnDeath = xp;
            objectHeight = height;
            objectWidth = width;

            resistances = gameObject.AddComponent<Resistances>();
            resistances.Init(resists);

            healthBar = HealthBarManager.instance.AddHealthBar(this);
            healthBar.Init(objectHeight, objectWidth);
        }


        //update health bar position automatically
        private void Update()
        {
            healthBar.OnUpdate();
        }


        //update health bar position manually
        public void OnUpdate()
        {
            healthBar.OnUpdate();
        }


        //delete health bar
        private void OnDestroy()
        {
            if (healthBar)
                Destroy(healthBar);
        }


        //changes the health value and gives xp to attacker if health goes below zero
        public void HealthChange(DamageType type, int damage, Attacker attacker = null)
        {
            //return if already below zero
            if (currentHealth <= 0)
                return;

            int previousHealth = currentHealth;

            //healing is not affected by resistance, and cannot exceed maximum health
            if ((damage < 0))
            {
                if (currentHealth - damage > maxHealth)
                    currentHealth = maxHealth;
                else
                    currentHealth -= damage;

                onHeal?.Invoke(gameObject, currentHealth, previousHealth);
            }
            //damage is rounded down after relevant resistance is removed
            else
            {
                int finalDamage = Mathf.FloorToInt(damage * resistances.DamageMultiplier(type));
                if (currentHealth - finalDamage <= 0)
                {
                    currentHealth = 0;
                    CombatManager.instance.MarkForDestruction(this);
                    if (attacker != null)
                        attacker.XPChange(xpOnDeath);

                    onDeath?.Invoke(gameObject);
                }
                else
                {
                    currentHealth -= finalDamage;

                    onDamage?.Invoke(gameObject, currentHealth, previousHealth);
                }
            }
        }


        [Client]
        //update health using value from the server
        public void SetHealth(int newHealth)
        {
            if (newHealth > currentHealth)
                onHeal?.Invoke(gameObject, newHealth, currentHealth);
            else if (newHealth > 0)
                onDamage?.Invoke(gameObject, newHealth, currentHealth);
            else
                onDeath?.Invoke(gameObject);
            
            
            if (currentHealth >= 0)
                currentHealth = newHealth;
        }


        [Client]
        //update visibility status
        public void SetVisibility(bool visible)
        {
            if (visible != isVisible)
                isVisible = visible;
        }


        //returns the attackables resistance values
        public List<int> GetResistances()
        {
            return resistances.GetResistanceValues();
        }
    }
}
