using System;
using UnityEngine;

public class Mana : MonoBehaviour, IManaUser
{
    [SerializeField, Min(0)] private float maxMana = 100f;
    [SerializeField, ReadOnly] private float currentMana;

    public float MaxMana => maxMana; 
    public float CurrentMana => currentMana;

    public event Action<float, float> OnManaChanged;

    private void Awake()
    {
        currentMana = maxMana;
    }

    void OnEnable()
    {
        currentMana = maxMana;
        OnManaChanged?.Invoke(currentMana, maxMana);
    }

    public void SetMaxMana(float manaVal)
    {
        maxMana = manaVal;
        currentMana = manaVal;
    }

    public void ConsumeMana(float manaVal)
    {
        if (manaVal <= 0) return;

        currentMana = Mathf.Max(currentMana - manaVal, 0f);
        OnManaChanged?.Invoke(currentMana, maxMana);
    }

    public void RestoreMana(float amount) { 
        if(amount < 0 ||  amount > maxMana) return;

        currentMana = Mathf.Min(currentMana + amount, maxMana);
        OnManaChanged?.Invoke(currentMana, maxMana);
    }
}
