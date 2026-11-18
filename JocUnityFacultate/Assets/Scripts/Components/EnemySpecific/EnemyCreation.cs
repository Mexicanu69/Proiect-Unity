using UnityEngine;

[RequireComponent(typeof(EnemyEntity))]
public class EnemyCreation : MonoBehaviour
{
    // daca pun inamicul de mana, ce date & target are
    [Header("Preset")]
    [SerializeField] private EnemyData presetData;
    [SerializeField] private Transform presetTarget;

    public EnemyData Data { get; private set; }

    private Health health;
    private Mover mover;
    private EnemyBrain brain;
    private bool isSetUp;
    private EnemyAutoAttack autoAttack;

    void Awake()
    {
        health = GetComponent<Health>();
        mover = GetComponent<Mover>();
        brain = GetComponent<EnemyBrain>();
        autoAttack = GetComponent<EnemyAutoAttack>();
    }

    void Start()
    {
        // inamicii spawnati de director sunt deja setup
        // pt cei pusi manual le dau setup cu preseturile din editor
        if (!isSetUp && presetData != null)
            Setup(presetData, presetTarget);
    }

    public void PlaceAt(Vector3 position)
    {
        if (mover != null)
            mover.Teleport(position);
        else
            transform.position = position;
    }

    public void Setup(EnemyData data, Transform target)
    {
        if (data == null)
        {
            Debug.LogError($"Am incercat sa fac un {name} fara EnemyData", this);
            return;
        }

        Data = data;
        health.SetMaxHealth(data.maxHealth);

        if (mover != null)
            mover.SetSpeed(data.moveSpeed);

        if (brain != null)
        {
            brain.SetAttackRange(data.attackRange);
            brain.SetTarget(target);
        }

        if (autoAttack != null)
            autoAttack.Setup(data.attackDamage, data.attackCooldown);

        isSetUp = true;
    }
}