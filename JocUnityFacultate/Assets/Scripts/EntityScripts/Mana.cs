using UnityEngine;

public class Mana : MonoBehaviour
{
    [SerializeField, Min(0)] private float maxMana = 100f;
    [SerializeField, ReadOnly] private float currentMana;

    public float MaxMana => maxMana; 
    public float CurrentMana => currentMana;

    private void Awake()
    {
        currentMana = maxMana;
    }

    public void SetMaxMana(float manaVal)
    {
        maxMana = manaVal;
        currentMana = manaVal;
    }

    public void ConsumeMana(float manaVal)
    {
        if (manaVal <= 0) return;

        if(currentMana >= manaVal)
        {
            currentMana -= manaVal;
        }

        if(currentMana < 0)
        {
            currentMana = 0;
        }
    }

}
