using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace DS_Resources
{
    [DisallowMultipleComponent]
    //this class contains everything to do with tracking and protecting players' resource counts
    //any change to values can only be called and executed by the server, but clients have a copy for faster/more efficient updates of the GUI
    public class ResourceManager : NetworkBehaviour
    {
        public static ResourceManager instance;
        
        [Tooltip("The resource data that will be used in this game")]
        public List<ResourceData> resources;
        [Tooltip("The amount of resources each player will start the game with")]
        public List<ResourceQuantity> initialResources;
        [Tooltip("The resource income that each player will start the game with")]
        public List<ResourceQuantity> initialIncome;

        [Tooltip("If false, the resource manager will reject any attempt to see resources that do not belong to that player. \nBest left false, but can be useful for a spying mechanic that allows the players to see the resource counts of other players.")]
        public bool canSeeAllResources = false;
        [Tooltip("Every time this amount of seconds passes, the income values will be added to the total resource counts")]
        public float incomeTickLength = 1.0f;

        //keep the master dictionaries private to force controlled interaction through methods
        private readonly SyncDictionary<uint, List<ResourceQuantity>> masterResourceDictionary = new SyncDictionary<uint, List<ResourceQuantity>>();
        private readonly SyncDictionary<uint, List<ResourceQuantity>> masterIncomeDictionary = new SyncDictionary<uint, List<ResourceQuantity>>();

        private Dictionary<ResourceType, uint> incomeTickValues;
        private uint totalResourceTicks = 0;


        //set up singleton
        void Awake()
        {
            if (instance != null && instance != this)
                Destroy(this);
            else
                instance = this;
        }


        //set the resource data externally
        public void Init(List<ResourceData> resourceData, List<ResourceQuantity> initResources, List<ResourceQuantity> initIncome)
        {
            resources = resourceData;
            initialResources = initResources;
            initialIncome = initIncome;

            if (isServer)
                StartCoroutine(IncomeCycle());

            incomeTickValues = new Dictionary<ResourceType, uint>();
            foreach (ResourceData resource in resourceData)
                incomeTickValues.Add(resource.resourceType, resource.ticksPerIncome);
        }


        [Server]
        //create both resource and income dictionaries for a new player
        public void CreateResourceDictionaries(uint owningPlayer)
        {
            //do not create a new dictionary if one exists
            if (masterResourceDictionary.ContainsKey(owningPlayer))
                return;

            //create lists of quantities for all resources and incomes
            List<ResourceQuantity> playerResources = new List<ResourceQuantity>();
            List<ResourceQuantity> playerIncome = new List<ResourceQuantity>();

            //add all resource types while checking for type duplicates
            foreach (ResourceData resource in resources)
            {
                int resourceIndex = playerResources.FindIndex(x => x.resourceType == resource.resourceType);
                if (resourceIndex == -1)
                    playerResources.Add(new ResourceQuantity() { resourceType = resource.resourceType, quantity = 0 });

                int incomeIndex = playerIncome.FindIndex(x => x.resourceType == resource.resourceType);
                if (incomeIndex == -1)
                    playerIncome.Add(new ResourceQuantity() { resourceType = resource.resourceType, quantity = 0 });
            }

            //set initial values
            foreach (ResourceQuantity value in initialResources)
            {
                int index = playerResources.FindIndex(x => x.resourceType == value.resourceType);
                if (index != -1)
                    playerResources[index] = value;
            }

            //set initial income
            foreach (ResourceQuantity value in initialIncome)
            {
                int index = playerResources.FindIndex(x => x.resourceType == value.resourceType);
                if (index != -1)
                    playerIncome[index] = value;
            }

            //add to master dictionaries using the matching player's ID as the key
            masterResourceDictionary.Add(owningPlayer, playerResources);
            masterIncomeDictionary.Add(owningPlayer, playerIncome);
        }


        //return the requested resource values with respect to canSeeAllResources
        public List<ResourceQuantity> GetResourceValues(uint requestedID, uint requesterID)
        {
            if (canSeeAllResources || requestedID == requesterID)
                if (masterResourceDictionary.ContainsKey(requestedID))
                    return masterResourceDictionary[requestedID];

            return null;
        }


        //return the requested income values with respect to canSeeAllResources
        public List<ResourceQuantity> GetIncomeValues(uint requestedID, uint requesterID)
        {
            if (canSeeAllResources || requestedID == requesterID)
                if (masterIncomeDictionary.ContainsKey(requestedID))
                    return masterIncomeDictionary[requestedID];

            return null;
        }


        //returns false if the requested change affects a different player, or any duplicate resourceTypes are found in the given array
        //can also check if the given resource change would set the master count negative
        //default arguement order - if negative count protection is desired, the function must specify if costs are given as positive or negative
        public bool IsResourceChangeValid(uint requestedID, uint requesterID, List<ResourceQuantity> resourceDeltas, bool costsArePositive = false, bool negativeCountProtection = false)
        {
            //check that the player requesting changes is the player who owns the resources that are being changed
            if (requestedID != requesterID)
            {
                Debug.LogError("Invalid resource change: Player is requesting change of another player's resources");
                return false;
            }

            //check for duplicate resourceTypes
            for (int i = 0; i <= resourceDeltas.Count; i++)
                for (int j = i + 1; j < resourceDeltas.Count; j++)
                    if (resourceDeltas[i].resourceType == resourceDeltas[j].resourceType)
                    {
                        Debug.LogError("Invalid resource change: Duplicate ResourceTypes found");
                        return false;
                    }

            //change is valid so far, return true if negative count protection is not requested
            if (!negativeCountProtection)
                return true;

            List<ResourceQuantity> playerResources = masterResourceDictionary[requestedID];

            //invert the sign
            int invert = -1;
            if (costsArePositive)
                invert = 1;

            //check that the changes do not cause any values to go negative
            foreach (ResourceQuantity resource in resourceDeltas)
                if (invert * resource.quantity > 0)
                {
                    float currentValue = playerResources.Find(x => x.resourceType == resource.resourceType).quantity;
                    if (currentValue < invert * resource.quantity)
                    {
                        Debug.LogWarning("Negative count protection: Player does not have enough resources for requested change");
                        return false;
                    }
                }

            //change is valid
            return true;
        }


        [Server]
        //attempts to adjust the given quantities to the resource count of the requested player and returns whether attempt was successful
        //the resources will only be changed if they belong to the player that originally requested the change
        //if the change would set any resource quantity negative, or any duplicate resourceTypes are found in the given array, none of the resources will be changed
        public bool OneOffResourceChange(uint requestedID, uint requesterID, List<ResourceQuantity> resourceChanges)
        {
            //check if values are valid with negative count protectino and costs as negative values
            if (!IsResourceChangeValid(requestedID, requesterID, resourceChanges, false, true))
                return false;

            //the requested changes are valid, change the master dictionary
            List<ResourceQuantity> playerResources = masterResourceDictionary[requestedID];
            for (int i = 0;i < resourceChanges.Count; i++)
            {
                int index = playerResources.FindIndex(x => x.resourceType == resourceChanges[i].resourceType);
                ResourceQuantity newQuantity = playerResources[index];
                newQuantity.quantity += resourceChanges[i].quantity;
                playerResources[index] = newQuantity;
            }
            masterResourceDictionary[requestedID] = playerResources;
            Debug.Log("Successful resource change");
            return true;
        }


        [Server]
        //updates the passive imcome by the given amount
        //the income will only be changed if they belong to the player that originally requested the change
        //there is no protection on income going negative
        public bool IncomeChange(uint requestedID, uint requesterID, List<ResourceQuantity> incomeChanges)
        {
            //the requested income change is valid, change the income dictionary
            List<ResourceQuantity> playerIncome = masterIncomeDictionary[requestedID];
            for (int i = 0; i < incomeChanges.Count; i++)
            {
                int index = playerIncome.FindIndex(x => x.resourceType == incomeChanges[i].resourceType);
                ResourceQuantity newCount = playerIncome[index];
                newCount.quantity += incomeChanges[i].quantity;
                playerIncome[index] = newCount;
            }
            masterIncomeDictionary[requestedID] = playerIncome;
            Debug.Log("Successful income change");
            return true;
        }


        [Server]
        //coroutine that increases all players' resources by their income amount at a set frequency
        IEnumerator IncomeCycle()
        {
            while (true)
            {
                yield return new WaitForSeconds(incomeTickLength);

                foreach (var pair in masterIncomeDictionary)
                {
                    List<ResourceQuantity> thisIncome = new List<ResourceQuantity>();

                    //add any income value that gets added this tick
                    foreach (ResourceQuantity resource in pair.Value)
                        if (totalResourceTicks % incomeTickValues[resource.resourceType] == 0)
                            thisIncome.Add(resource);

                    if (thisIncome.Count == 0)
                    {
                        Debug.Log("No income to add");
                        yield break;
                    }

                    //process income and handle errors
                    if (!OneOffResourceChange(pair.Key, pair.Key, thisIncome))
                    {
                        //TBC// add events to handle negative resource count
                        Debug.LogWarning("Income could not be added to player ID: " + pair.Key.ToString() + ", at least one resource would become negative");
                    }
                    else
                    {
                        Debug.Log("Income added");
                    }
                }
            }
        }

    }
}
