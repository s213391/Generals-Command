using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Moveable : MonoBehaviour
{
    NavMeshAgent agent;
    float moveSpeed = 1.0f;
    float acceleration = 1.0f;
    float angularSpeed = 120.0f;
    float height = 2.0f;
    float width = 0.5f;

    int pathfindingPriority = 50;
    bool passThroughOtherAgents = false;

    bool movementEnabled = true;
    Transform destination;
    float forwardFacing = 0.0f;
    float rightFacing = 0.0f;

    MovableEvents movableEvents;
    
    
    // Start is called before the first frame update
    void Init()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!agent)
            agent = gameObject.AddComponent<NavMeshAgent>();
    }


    // Update is called once per frame
    void Update()
    {
        Vector3 movementVector = agent.desiredVelocity.normalized;

        if (movementVector == Vector3.zero)
            return;

        forwardFacing = Vector3.Dot(movementVector, transform.forward);
        rightFacing = Vector3.Dot(movementVector, transform.right);

        agent.speed = Mathf.Clamp(forwardFacing, 0.0f, 1.0f) * moveSpeed;

        movableEvents.OnUpdate(forwardFacing, rightFacing);
    }
}
