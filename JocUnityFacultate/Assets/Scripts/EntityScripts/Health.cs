using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField, Min(1)] private float maxHealth = 100f;
    [SerializeField, ReadOnly] private float currentHealth;
    [SerializeField, ReadOnly] private bool isDead;

    // readonly public
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsDead => isDead;

    public event Action OnDeath;

    void Awake()
    {
        currentHealth = maxHealth;    
    }

    public void SetMaxHealth(float valMaxHp)
    {
        maxHealth = valMaxHp;
        currentHealth = valMaxHp;
        isDead = false;
    }

    public void TakeDamage(float amt)
    {
        if (isDead || amt <= 0)
            return;

        currentHealth -= amt;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void HealDamage(float amt)
    {
        if (isDead || amt <= 0)
            return;

        if (currentHealth + amt >= maxHealth)
            // nu depaseste max hp
            currentHealth = maxHealth;
        else
            currentHealth += amt;
    }

    private void Die()
    {
        isDead = true;
        // daca are un OnDeath apeleaza-l
        OnDeath?.Invoke();
    }

    [ContextMenu("Damage Test")]
    private void TestDamage()
    {
        TakeDamage(15);
        Debug.Log($"{name} TakeDamage(15): {currentHealth}/{maxHealth}");
        HealDamage(10);
        Debug.Log($"{name} HealDamage(10): {currentHealth}/{maxHealth}");
    }
}
