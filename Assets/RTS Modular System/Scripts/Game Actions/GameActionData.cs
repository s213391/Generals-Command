using System.Collections.Generic;
using UnityEngine;
using DS_Resources;

namespace RTSModularSystem
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Action Data", order = 102)]
    [System.Serializable]
    public class GameActionData : ScriptableObject
    {
        [Header("Settings")]
        [Tooltip("The time after the end of the action that this action can not be reused")]
        public float cooldown;
        [Tooltip("If this action is run on the client, or on the server. \nAny action that affects resources can only be run on the server")]
        public bool clientSide; 
        [Tooltip("If this action prevents other actions from tracking inputs until it ends")]
        public bool lockInput;
        [Tooltip("If this action will appear as a button on the action bar when its player object is selected")]
        public bool showOnActionBar;
        [Tooltip("The image that will be displayed in the GUI representing this action")]
        public Sprite icon;
        [Range(1, 9)] [Tooltip("The value of interruption that is required to stop this action early. \nDefault interruption values: \n1: Starting a new action/ deselecting a player object \n3: The stop actions button \n8: 'Killing' the Player Object performing this action \n9: Destroying the Player Object script")]
        public int interruptionTolerance = 1;

        [Header("Action Effects")]
        [Tooltip("GameObjects that will be spawned at the start of this action")]
        public List<ObjectCreation> objectsToSpawn;
        [Tooltip("This will not affect the players resources, but prevents the action starting until the player has more resources than this threshold")]
        public List<ResourceQuantity> mustHaveTheseResources;
        [Tooltip("Any changes to player resources this action causes. \nPositive quantities for production or bonuses, Negative quantities for costs")]
        public List<ResourceQuantity> resourceChange;

        [Header("Ending the Action")]
        [Tooltip("If this action will restart itself every time it ends with a success state")]
        public bool loopOnSuccessfulEnd;
        [Tooltip("The conditions that will immediately end this action. \nEach condition is treated as either a success or a failure")]
        public List<ActionEnd> endConditions;
        [Tooltip("Conditions that will not end the action when they are true, but must all be true when an end condition occurs for the action to end as a success")]
        public List<ActionEnd> successConditions;
        [Tooltip("Actions that will start when this action ends with a success state")]
        public List<GameActionData> nextActionsOnSuccess;
        [Tooltip("Actions that will start when this action ends with a failure state")]
        public List<GameActionData> nextActionsOnFailure;
    }
}
