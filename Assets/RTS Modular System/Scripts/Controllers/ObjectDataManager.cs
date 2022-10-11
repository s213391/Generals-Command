using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using DS_Resources;

namespace RTSModularSystem
{
    [System.Serializable]
    //used as a temporary storage for object data that is visible in the inspector, before converting into dictionary at runtime
    public struct DictionaryInInspector
    {
        public PlayerObjectType type;
        public List<PlayerObjectData> data;
    }


    //used to hold and access all PlayerObjectData used by the game
    //it is critical that all players have the same data and in the same order
    //this data is highly inefficient to send over the network, so only the type and index are sent, and then accessed on the other side
    public class ObjectDataManager : MonoBehaviour
    {
        public static ObjectDataManager instance;

        [Header("Host Spawn")]
        [SerializeField]
        private Vector3 hostPosition;
        [SerializeField]
        private Vector3 hostRotation;

        [Header("Client Spawn")]
        [SerializeField]
        private Vector3 clientPosition;
        [SerializeField]
        private Vector3 clientRotation;

        [Header("Replaceable Colour")]
        [SerializeField] [Tooltip("The colour of the replaceable material will be changed to this when the game starts. \nUseful to make replaceable be an obvious colour in editor, but more subtle in game")]
        private Color inGameColour = new Color(79.0f / 255.0f, 79.0f / 255.0f, 79.0f / 255.0f);
        private Color originalReplaceable;
        private Material replaceable;

        public Dictionary<PlayerObjectType, List<PlayerObjectData>> objects = new Dictionary<PlayerObjectType,List<PlayerObjectData>>();

        [SerializeField]
        [Tooltip("Place all PlayerObjectData here in the list that has the same type")]
        private List<DictionaryInInspector> objectData;

        [Tooltip("Place a ResourceData here for every unique resource")]
        public List<ResourceData> resources;
        [Tooltip("The resources that each player will start with")]
        public List<ResourceQuantity> initialResources;
        [Tooltip("The resource income that each player will start with")]
        public List<ResourceQuantity> initialIncome;

        //holds reference to every created player object separated into owning player and then data name
        private Dictionary<uint, Dictionary<string, List<PlayerObject>>> allPlayerObjects = new Dictionary<uint, Dictionary<string, List<PlayerObject>>>();


        //Ensure only one ObjectDataManager exists, then convert objectData into dictionary
        void Awake()
        {
            if (instance != null && instance != this)
                Destroy(this);
            else
                instance = this;
        }


        //set up object and resource data as well as the replaceable material
        public void Init()
        {
            replaceable = Resources.Load<Material>("Replaceable");
            originalReplaceable = replaceable.color;
            replaceable.color = inGameColour;

            foreach (DictionaryInInspector dii in objectData)
                if (!objects.ContainsKey(dii.type))
                    objects.Add(dii.type, dii.data);
                else
                    objects[dii.type].AddRange(dii.data);

            ResourceManager.instance.Init(resources, initialResources, initialIncome);
        }


        //undo replaceable colour change
        private void OnDestroy()
        {
            replaceable.color = originalReplaceable;
        }


        //returns the index of the matching data, -1 if no match
        public int GetObjectIndex(PlayerObjectData data)
        {
            List<PlayerObjectData> list;
            if(objects.TryGetValue(data.objectType, out list))
                return list.IndexOf(data);

            return -1;
        }


        //returns the data at the given index, null if out of bounds
        public PlayerObjectData GetObjectData(PlayerObjectType type, int index)
        {
            List<PlayerObjectData> list;
            if (objects.TryGetValue(type, out list))
                if (list.Count > index)
                    return list[index];

            return null;
        }


        //returns the index of the matching resource, -1 if no match
        public int GetResourceIndex(ResourceData data)
        {
            return resources.IndexOf(data);
        }


        //returns the resource at the given index, null if out of bounds
        public ResourceData GetResourceData(int index)
        {
            if (resources.Count > index)
                return resources[index];
            return null;
        }


        public static Vector3 HostPosition()
        {
            return instance.hostPosition;
        }


        public static Vector3 HostRotation()
        {
            return instance.hostRotation;
        }


        public static Vector3 ClientPosition()
        {
            return instance.clientPosition;
        }


        public static Vector3 ClientRotation()
        {
            return instance.clientRotation;
        }


        public static void CreateNewPlayerObjectDictionary(uint playerID)
        {
            if (instance.allPlayerObjects.ContainsKey(playerID))
                return;

            instance.allPlayerObjects.Add(playerID, new Dictionary<string, List<PlayerObject>>());
            foreach (KeyValuePair<PlayerObjectType, List<PlayerObjectData>> kvPair in instance.objects)
                foreach (PlayerObjectData poData in kvPair.Value)
                    instance.allPlayerObjects[playerID].Add(poData.name, new List<PlayerObject>());

            Debug.Log("ObjectDataManager: New player dictionary created: " + playerID.ToString());
        }


        public static void AddPlayerObject(uint playerID, PlayerObject po)
        {
            Debug.Log("ObjectDataManager: Adding new object: " + playerID.ToString() + ", " + po.data.name);

            if (!instance.allPlayerObjects.ContainsKey(playerID))
                CreateNewPlayerObjectDictionary(playerID);

            if (instance.allPlayerObjects[playerID].ContainsKey(po.data.name))
                instance.allPlayerObjects[playerID][po.data.name].Add(po);
            else
                Debug.Log("ObjectDataManager: Trying to create player object type that does not exist in dictionary: " + playerID.ToString() + ", " + po.data.name);

            if (instance.allPlayerObjects[playerID].ContainsKey(po.data.name) && instance.allPlayerObjects[playerID][po.data.name].Contains(po))
                Debug.Log("ObjectDataManager: Successfully added new object: " + playerID.ToString() + ", " + po.data.name);
            else
                Debug.Log("ObjectDataManager: Error, object not added to dictionary");
        }


        public static List<PlayerObject> GetPlayerObjectsOfType(string typeName, uint playerID = 0)
        {
            //return all objects of type, regardless of ownership
            if (playerID == 0)
            {
                List<PlayerObject> objectsOfType = new List<PlayerObject>();
                foreach (KeyValuePair<uint, Dictionary<string, List<PlayerObject>>> kvPair in instance.allPlayerObjects)
                {
                    if (kvPair.Value.ContainsKey(typeName))
                        objectsOfType.AddRange(kvPair.Value[typeName]);
                    else
                        Debug.Log("ObjectDataManager: Trying to retrieve player object type that does not exist in dictionary: " + kvPair.Key.ToString() + ", " + typeName);
                }
                return objectsOfType;
            }
            //return all objects of type that are owned by the given player
            else if (instance.allPlayerObjects.ContainsKey(playerID))
            {
                if (instance.allPlayerObjects[playerID].ContainsKey(typeName))
                    return instance.allPlayerObjects[playerID][typeName];
                else
                    Debug.Log("ObjectDataManager: Trying to retrieve player object type that does not exist in dictionary: " + playerID.ToString() + ", " + typeName);
            }

            //player doesn't exist, return null
            Debug.Log("ObjectDataManager: Trying to retrieve player object from player not in dictionary: " + playerID.ToString());
            return null;
        }


        public static void RemovePlayerObject(uint playerID, PlayerObject po)
        {
            Debug.Log("ObjectDataManager: Removing new object: " + playerID.ToString() + ", " + po.data.name);

            if (instance.allPlayerObjects.ContainsKey(playerID))
            {
                if (instance.allPlayerObjects[playerID].ContainsKey(po.data.name))
                    instance.allPlayerObjects[playerID][po.data.name].Remove(po);
                else
                    Debug.Log("ObjectDataManager: Trying to remove player object type that does not exist in dictionary");
            }
            else
                Debug.Log("ObjectDataManager: Trying to remove player object from player not in dictionary");
        }
    }
}
