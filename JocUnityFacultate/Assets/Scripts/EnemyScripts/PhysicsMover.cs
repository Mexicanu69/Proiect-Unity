using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class PhysicsMover : Mover
{
    [SerializeField, Min(0)] private float acceleration = 40f;
    [SerializeField, Min(0)] private float turnSpeed = 720f;

    private NavMeshAgent agent;
    private Rigidbody rb;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        agent.updatePosition = false;
        agent.updateRotation = false;
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
        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
    }

    public override void Teleport(Vector3 position)
    {
        rb.position = position;
        transform.position = position;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (agent.enabled)
            agent.Warp(position);
    }

    void FixedUpdate()
    {
        // unde vrei sa mergi (default 0 esti oprit)
        Vector3 desired = Vector3.zero;
        // daca nu esti oprit si n-ai iesit din nav mesh iei viteza
        if (agent.isOnNavMesh && !agent.isStopped)
            desired = agent.desiredVelocity;

        // calculeaza cat te misti
        Vector3 change = desired - rb.linearVelocity;
        change.y = 0f; // nu te misti deloc pe axa Oy
        change = Vector3.ClampMagnitude(change, acceleration * Time.fixedDeltaTime);
        // aplica forta care te misca
        rb.AddForce(change, ForceMode.VelocityChange);

        Vector3 lookAt = new Vector3(desired.x, 0f, desired.z); // in ce directie se uita
        if (lookAt.sqrMagnitude > 0.01f)
        {
            // quaternion care se uita in directia lookAt
            Quaternion look = Quaternion.LookRotation(lookAt);
            // uita-te unde iti zice quaternionul
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, look, turnSpeed * Time.fixedDeltaTime));
        }

        agent.nextPosition = rb.position;
    }
}