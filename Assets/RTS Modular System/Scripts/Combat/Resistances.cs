using System.Collections.Generic;
using UnityEngine;

namespace RTSModularSystem.BasicCombat
{
    [System.Serializable]
    //how resistant an attackable is to damage of this type
    public struct DamageResistance
    {
        public DamageType damageType;
        public int percentageResistance;
    }


    //Class that takes a list of resistances (inspector-editable) and turns it into a dictionary (not inspector-editable)
    public class Resistances : MonoBehaviour
    {
        //holds each resistance value at runtime
        private Dictionary<DamageType, int> resistanceDictionary;


        //turn list into dictionary
        public void Init(List<DamageResistance> resistances)
        {
            resistanceDictionary = new Dictionary<DamageType, int>();

            foreach (DamageResistance resistance in resistances)
                if (!resistanceDictionary.ContainsKey(resistance.damageType))
                    resistanceDictionary.Add(resistance.damageType, resistance.percentageResistance);
        }


        //returns the multiplier of damage taken of the given damage type
        public float DamageMultiplier(DamageType type)
        {
            if (resistanceDictionary.ContainsKey(type))
                return 1 - resistanceDictionary[type] / 100.0f;
            else
                return 1;
        }


        //modifies the resistance value of the given type by the given amount
        public void ChangeResistance(DamageType type, int change)
        {
            if (!resistanceDictionary.ContainsKey(type))
                return;

            //change the resistance but keep within [0, 100]
            resistanceDictionary[type] = Mathf.Clamp(resistanceDictionary[type] + change, 0, 100);
        }


        //returns the resistance values in an ordered list
        public List<int> GetResistanceValues()
        { 
            List<int> values = new List<int>();

            for (int i = 0; i < resistanceDictionary.Count; i++)
            {
                values.Add(resistanceDictionary[(DamageType)i]);
            }
            return values;
        }
    }
}
