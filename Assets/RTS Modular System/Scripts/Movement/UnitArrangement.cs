using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

namespace RTSModularSystem
{
    public class UnitArrangement : MonoBehaviour
    {
        [Tooltip("The buffer space between units' colliders at the destination when multiple units are given the same movement/combat order.")]
        public float unitSpacing;

        [Tooltip("The maximum height difference allowed between the clicked position and the destinations set")]
        public float heightTolerance;

        List<float> spaceLeftInRing; //the amount of the ring's perimeter that is not occupied left
        List<float> ringRadii; //each ring's radius
        List<int> ringAngleOffset; //a randomly selected angle offset to break up the pattern

        //assigns different, valid destinations in rings around a centrepoint to every given agent
        //each ring will be the size of the previous ring, plus unit spacing and the radii of the largest agents from each of the two rings
        //the centrepoint can be another agent whose size will determine the radius of the first ring
        //must specify if target is enemy or not to use a target, if an enemy, ranged units' destinations will be at their max range away from the enemy
        public void AssignDestination(List<NavMeshAgent> agents, Vector3 centrepoint, bool isTargetAnEnemy = false, PlayerObject target = null)
        {
            //if there is a target, the centrepoint does not need to be (and likely isn't) on the navmesh
            if (target == null)
            {
                //do not set any destinations if there is no target and the given point is off the navmesh
                if (!NavMesh.SamplePosition(centrepoint, out NavMeshHit centreHit, 0.2f, NavMesh.AllAreas) || agents.Count == 0)
                    return;
                else
                    //make sure the centrepoint is on the navmesh
                    centrepoint = centreHit.position;
            }
            else
            {
                centrepoint = target.gameObject.transform.position;
            }

            //set up target
            NavMeshAgent targetAgent = target?.gameObject.GetComponent<NavMeshAgent>();
            NavMeshObstacle targetObstacle = target?.gameObject.GetComponent<NavMeshObstacle>();

            spaceLeftInRing = new List<float>();
            ringRadii = new List<float>();
            ringAngleOffset = new List<int>();

            //sort list of agents, largest to smallest
            agents.Sort((y, x) => x.radius.CompareTo(y.radius));

            //initialise first ring based on whether a target exists
            int loopStart;
            if (target != null)
            {
                //create zeroth ring
                ringRadii.Add(0);
                spaceLeftInRing.Add(0);
                ringAngleOffset.Add(0);

                //set radius of first ring based on target size
                if (targetAgent)
                    ringRadii.Add(targetAgent.radius + unitSpacing);
                else if (targetObstacle)
                    ringRadii.Add(targetObstacle.radius + unitSpacing);

                loopStart = 0;
            }
            else
            {
                RTSPlayer.localPlayer.CmdMoveUnit(agents[0].gameObject, centrepoint);

                //if there is only one agent and no target, there is no need for calculating rings
                if (agents.Count == 1)
                    return;

                //create zeroth ring
                ringRadii.Add(0);
                spaceLeftInRing.Add(0);
                ringAngleOffset.Add(0);

                //set radius of first ring based on the largest agent's size
                ringRadii.Add(agents[0].radius + unitSpacing);
                loopStart = 1;
            }

            //set up rest of first ring based on the radius
            spaceLeftInRing.Add(ringRadii[0] * 2 * Mathf.PI);
            ringAngleOffset.Add(Random.Range(0, 360));

            //set the destination of each agent in rings expanding out from the centrepoint
            for (int i = loopStart; i < agents.Count; i++)
            {
                int attemptCount = 0;
                //choose a ring and position until one is found that is on the navmesh
                while (true)
                {
                    //crash protection
                    attemptCount++;
                    if (attemptCount > 101)
                    {
                        Debug.LogError("Pathfinding has made over 100 attempts to find a valid destination for this unit and has given up");
                        spaceLeftInRing.Clear();
                        ringRadii.Clear();
                        ringAngleOffset.Clear();
                        return;
                    }

                    int ring = ChooseRing(agents[i]);
                    Vector3 position = centrepoint + ChoosePosition(agents[i], ring);

                    if (NavMesh.SamplePosition(position, out NavMeshHit agentHit, heightTolerance, agents[i].areaMask))
                    {
                        Vector3 samplePosition = agentHit.position;

                        //use the sample position for the destination, but only if the x and z values are close to the assigned position to avoid overlapping agents
                        if (Mathf.Abs(position.x - samplePosition.x) > 0.5f * unitSpacing || Mathf.Abs(position.z - samplePosition.z) > 0.5f * unitSpacing)
                            continue;

                        RTSPlayer.localPlayer.CmdMoveUnit(agents[i].gameObject, agentHit.position);
                        break;
                    }
                }
            }

            //cleanup
            spaceLeftInRing.Clear();
            ringRadii.Clear();
            ringAngleOffset.Clear();
        }


        //returns the index of the first ring with enough space to hold an agent of the given size
        //checks if the second largest ring has enough space remaining, if not, will create a new ring and set the previous second largest to no space remaining
        private int ChooseRing(NavMeshAgent agent)
        {
            int largestIndex = spaceLeftInRing.Count - 1;
            if (spaceLeftInRing[largestIndex - 1] >= 2 * agent.radius + unitSpacing)
                return largestIndex - 1;

            //second largest ring does not have enough space, adjust largest ring size and create space for agents
            spaceLeftInRing[largestIndex - 1] = 0;
            ringRadii[largestIndex] += agent.radius;
            spaceLeftInRing[largestIndex] = ringRadii[largestIndex] * 2 * Mathf.PI;

            //create new larger ring but do not calculate circumference yet as we do not know yet what the largest agent in this ring will be
            ringRadii.Add(ringRadii[largestIndex] + unitSpacing + agent.radius);
            ringAngleOffset.Add(Random.Range(0, 360));
            spaceLeftInRing.Add(0);

            return largestIndex;
        }


        //returns the position, relative to the centrepoint this agent would fit within the given ring
        private Vector3 ChoosePosition(NavMeshAgent agent, int ring)
        {
            //get the ring's circumference and the angle of this position
            float circumference = ringRadii[ring] * 2 * Mathf.PI;
            float angleToPosition = 360.0f * (spaceLeftInRing[ring] - agent.radius) / circumference;

            //remove the space this agent would occupy from the ring
            spaceLeftInRing[ring] -= (2 * agent.radius + unitSpacing);

            Vector3 radius = new Vector3(0.0f, 0.0f, ringRadii[ring]);
            Quaternion angle = Quaternion.Euler(0.0f, angleToPosition + ringAngleOffset[ring], 0.0f);
            return angle * radius;
        }


        [Server]
        //server command to move units
        public void MoveUnit(GameObject unit, Vector3 destination)
        {
            unit.GetComponent<NavMeshAgent>().SetDestination(destination);
        }
    }
}
