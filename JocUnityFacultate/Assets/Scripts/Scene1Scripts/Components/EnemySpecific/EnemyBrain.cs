using UnityEngine;

public abstract class EnemyBrain : MonoBehaviour
{
    // follow target (pe cine urmaresc)
    [SerializeField] protected Transform target;

    // luat din enemyData
    [SerializeField, Min(0)] protected float attackRange = 1.5f;

    // daca e prea mic risca sa crape jocul ca recalculeaza prea mult
    [SerializeField, Min(0.05f)] private float repathInterval = 0.2f;

    [SerializeField, Min(0)] private float turnSpeed = 720f;

    protected Collider targetCollider;

    protected Health targetHealth;
    protected EnemyAutoAttack autoAttack;

    protected Mover mover;
    private float repathTimer; // cat mai face pana la urmatorul repath

    private bool isMoving;

    protected virtual void Awake()
    {
        mover = GetComponent<Mover>();

        if (mover == null)
            Debug.LogError($"{name} are EnemyBrain fara Mover si nu se va misca deloc", this);

        autoAttack = GetComponent<EnemyAutoAttack>(); // pot sa pun null caz in care inamicul nu mai are AA uri
    }

    // Ruleaza de fiecare data cand inamicul e activat,
    // inclusiv cand e refolosit din pool: uitam starea din viata trecuta
    protected virtual void OnEnable()
    {
        isMoving = false;
        repathTimer = 0f;
    }
    protected virtual void Start()
    {
        if (target != null && targetHealth == null)
            SetTarget(target);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        targetHealth = newTarget != null ? newTarget.GetComponent<Health>() : null;
        targetCollider = newTarget != null ? newTarget.GetComponentInChildren<Collider>() : null;
    }

    public void SetAttackRange(float range)
    {
        attackRange = range;
    }

    protected bool IsTargetInRange()
    {
        Vector3 point = targetCollider != null
        ? targetCollider.ClosestPoint(transform.position)
        : target.position;

        Vector3 offset = point - transform.position;
        offset.y = 0f;
        return offset.sqrMagnitude <= attackRange * attackRange;
    }

    // la un moment dat, unde ne ducem?
    // fiecare tip de creier va face diferit
    // ex Chaser merge in linie dreapta, dar Kiter poate se duce in and out
    protected abstract Vector3 GetDestination();
    private void FaceTarget()
    {
        Vector3 direction = target.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        Quaternion look = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, look, turnSpeed * Time.deltaTime);
    }

    protected void Update()
    {
        if (target == null || mover == null) 
            return;

        if (IsTargetInRange())
        {
            if (isMoving)
            {
                mover.Stop();
                isMoving = false;
            }
            // primesti un repath imediat 
            // de ex pentru Kiter il ajuta
            repathTimer = 0f;

            // autoattack

            FaceTarget();

            if (autoAttack != null)
                autoAttack.TryAttack(targetHealth);

            return;
        }

        isMoving = true;
        repathTimer -= Time.deltaTime;

        if (repathTimer <= 0f)
        {
            // am asteptat suficient, fa repath
            repathTimer = repathInterval;
            // si poti sa mergi la el
            mover.MoveTo(GetDestination());
        }
    }
}