using UnityEngine;

public class EnemyAutoAttack : MonoBehaviour
{
    [SerializeField, Min(0)] private float damage = 5f;
    [SerializeField, Min(0.05f)] private float cooldown = 1f;

    // in ce moment poate ataca din nou
    private float readyTime;

    public bool IsReady => isActiveAndEnabled && Time.time >= readyTime;

    void OnEnable()
    {
        // daca era in pool si era pe cooldown vine pregatit de atac
        readyTime = 0f;
    }

    public void Setup(float newDamage, float newCooldown)
    {
        damage = newDamage;
        cooldown = Mathf.Max(0.05f, newCooldown);
    }

    public bool TryAttack(Health targetHealth) // fa un atac. a mers atacul?
    {
        if (!IsReady || targetHealth == null || targetHealth.IsDead)
            return false;

        targetHealth.TakeDamage(damage);

        readyTime = Time.time + cooldown;

        // atacul a mers
        return true;
    }
}