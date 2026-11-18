using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshMover : Mover
{
    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public override void SetSpeed(float speed)
    {
        agent.speed = speed;
    }

    public override void MoveTo(Vector3 destination)
    {
        if (!agent.isOnNavMesh) 
            return;

        agent.isStopped = false;
        agent.SetDestination(destination);
    }

    public override void Stop()
    {
        if (!agent.isOnNavMesh) 
            return;

        agent.isStopped = true;
        agent.ResetPath();
        agent.velocity = Vector3.zero;
    }

    public override void Teleport(Vector3 position)
    {
        if (agent.enabled && agent.Warp(position))
            return;

        transform.position = position;
    }
}
