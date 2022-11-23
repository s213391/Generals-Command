using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RTSModularSystem;
using Mirror;

public class Moveable : MonoBehaviour
{
    public NavMeshAgent agent;
    public float moveSpeed = 1.0f;
    public float acceleration = 1.0f;
    public float angularSpeed = 120.0f;
    public float height = 2.0f;
    public float width = 0.5f;
    public int pathfindingPriority = 50;
    public bool passThroughOtherAgents = false;

    public float secondsBetweenFollowingUpdates = 1.0f;

    public MovableEvents movableEvents;

    public Transform destinationCollider = null;
    public Transform followTarget = null;

    bool movementEnabled;
    bool isMoving;
    List<Moveable> friendlyFollowingObjects;
    List<Moveable> enemyFollowingObjects;
    bool followingUpdatedRecently = false;
    float forwardFacing = 0.0f;
    float rightFacing = 0.0f;
    
    
    //set up the movable component and all required components
    public void Init(float speed, float accel, float angular, float agentHeight, float agentWidth, int priority, bool passThrough)
    {
        moveSpeed = speed;
        acceleration = accel;
        angularSpeed = angular;
        height = agentHeight;
        width = agentWidth;
        pathfindingPriority = priority;
        passThroughOtherAgents = passThrough;

        movableEvents = GetComponent<MovableEvents>();

        if (!GameData.instance.isHost)
            return;

        agent = GetComponent<NavMeshAgent>();
        if (!agent)
            agent = gameObject.AddComponent<NavMeshAgent>();

        movementEnabled = true;
        isMoving = false;

        friendlyFollowingObjects = new List<Moveable>();
        enemyFollowingObjects = new List<Moveable>();

        destinationCollider = Instantiate((GameObject)Resources.Load("Destination Collider")).transform;
        destinationCollider.localScale = Vector3.one * 2 * width;
        NetworkServer.Spawn(destinationCollider.gameObject);

        InitialiseAgent();
        StartCoroutine(FollowingUpdateCooldownDuration());
    }


    //takes the stored values to set up the navmesh agent
    void InitialiseAgent()
    {
        agent.speed = moveSpeed;
        agent.acceleration = acceleration;
        agent.angularSpeed = angularSpeed;
        agent.height = height;
        agent.radius = width;
        agent.avoidancePriority = pathfindingPriority;
        agent.autoTraverseOffMeshLink = false;

        if (passThroughOtherAgents)
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        else
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;
    }


    //update speed and movement animations
    public void OnUpdate()
    {
        if (!GameData.instance.isHost)
            return;

        if (isMoving && movementEnabled)
        {
            if (!agent.pathPending)
            {
                Vector3 movementVector = agent.velocity.normalized;
                if (movementVector == Vector3.zero)
                    movementVector = agent.desiredVelocity.normalized;

                forwardFacing = Vector3.Dot(movementVector, transform.forward);
                rightFacing = Vector3.Dot(movementVector, transform.right);

                //if moving backwards, set turning to full and movement to 0
                if (forwardFacing < 0)
                {
                    forwardFacing = 0;
                    rightFacing /= Mathf.Abs(rightFacing);
                }

                if (!followingUpdatedRecently)
                    UpdateFollowingObjects();

                movableEvents?.OnUpdate(forwardFacing, rightFacing);

                agent.speed = Mathf.Clamp(forwardFacing, 0.05f, 1.0f) * moveSpeed;

                if (agent.remainingDistance <= agent.stoppingDistance)
                { 
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0.0f)
                    {
                        isMoving = false;
                        movableEvents.OnMovementEnd();
                        UpdateFollowingObjects();
                    }
                }
            }
        }
    }


    //sets the agents destination and handles events
    public void SetDestination(Vector3 dest, bool playEvent, bool clearTarget = true)
    {
        agent.SetDestination(dest);
        destinationCollider.position = dest;
        isMoving = true;

        if (clearTarget && followTarget)
        {
            Moveable oldTarget = followTarget.GetComponent<Moveable>();
            if (oldTarget)
                oldTarget.RemoveTargetted(this);
        }

        movableEvents.OnMovementBegin();
    }


    //sets a target for this object to move towards and follow
    public void SetTarget(Transform target, Vector3 dest, bool friendly, bool playEvent)
    {
        if (followTarget)
        {
            Moveable oldTarget = followTarget.GetComponent<Moveable>();
            if (oldTarget)
                oldTarget.RemoveTargetted(this);
        }

        followTarget = target;
        Moveable moveable = target.GetComponent<Moveable>();
        if (moveable)
            moveable.SetTargetted(this, friendly);

        SetDestination(dest, playEvent, false);
    }


    //tells a movable that it has been targetted
    public void SetTargetted(Moveable moveable, bool friendly)
    {
        if (friendly)
        {
            if (!friendlyFollowingObjects.Contains(moveable))
                friendlyFollowingObjects.Add(moveable);
        }
        else
        {
            if (!enemyFollowingObjects.Contains(moveable))
            enemyFollowingObjects.Add(moveable);
        }
    }


    //removes a moveable from the following list
    public void RemoveTargetted(Moveable moveable)
    {
        if (RTSPlayer.Owns(moveable.GetComponent<PlayerObject>()))
            friendlyFollowingObjects.Remove(moveable);
        else
            enemyFollowingObjects.Remove(moveable);
    }


    //tells all following moveables where this moveable is now
    void UpdateFollowingObjects()
    {
        followingUpdatedRecently = true;
        
        if (friendlyFollowingObjects.Count > 0)
            RTSPlayer.unitArrangement.AssignDestination(friendlyFollowingObjects, transform.position, false, false, GetComponent<PlayerObject>());
        if (enemyFollowingObjects.Count > 0)
            RTSPlayer.unitArrangement.AssignDestination(enemyFollowingObjects, transform.position, false, true, GetComponent<PlayerObject>());
    }


    //allow follow target checking after duration has elapsed
    private IEnumerator FollowingUpdateCooldownDuration()
    {
        while (true)
        {
            yield return new WaitForSeconds(secondsBetweenFollowingUpdates);
            followingUpdatedRecently = false;
        }
    }


    private void OnDestroy()
    {
        Destroy(agent);
    }
}
