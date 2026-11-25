using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField, Min(1)] private float maxHealth = 100f;
    [SerializeField, ReadOnly] private float currentHealth;
    [SerializeField, ReadOnly] private bool isDead;
    //listener pentru damage
    public event Action<float, float> OnHealthChange;
    public event Action<float> OnDamaged;

    // readonly public
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsDead => isDead;

    public event Action OnDeath;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void OnEnable()
    {
        currentHealth = maxHealth;
        isDead = false;
        OnHealthChange?.Invoke(currentHealth, maxHealth);
    }

    public void SetMaxHealth(float valMaxHp)
    {
        maxHealth = valMaxHp;
        currentHealth = valMaxHp;
        isDead = false;
        OnHealthChange?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float amt)
    {
        if (isDead || amt <= 0)
            return;

        currentHealth -= amt;
        OnHealthChange?.Invoke(currentHealth, maxHealth);
        OnDamaged?.Invoke(amt);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void GainHealth(float amt)
    {
        if (isDead || amt <= 0)
            return;

        if (currentHealth + amt >= maxHealth)
        {
            // nu depaseste max hp
            currentHealth = maxHealth;
            OnHealthChange?.Invoke(currentHealth, maxHealth);
        }
        else
        {
            currentHealth += amt;
            OnHealthChange?.Invoke(currentHealth, maxHealth);
        }
    }

    private void Die()
    {
        isDead = true;
        // daca are un OnDeath apeleaza-l
        OnDeath?.Invoke();
    }
}
