using UnityEngine;
using UnityEngine.UI;

namespace RTSModularSystem.GameResources
{
    //replace these with the names of every resource used in the game
    public enum ResourceType
    {
        Money,
        Scrap,
        Oil,
        MechanicalParts,
        Uranium
    }


    //used for tracking costs and updating resource counts
    [System.Serializable]
    public struct ResourceQuantity
    {
        public ResourceType resourceType;
        public int quantity;
    }


    //used to show a matching sprite in GUI elements
    [CreateAssetMenu(menuName = "Scriptable Objects/Resource Data", order = 103)]
    [System.Serializable]
    public class ResourceData : ScriptableObject
    {
        public ResourceType resourceType;
        public uint ticksPerIncome = 1;
        public Sprite guiSprite; 
    }
}
