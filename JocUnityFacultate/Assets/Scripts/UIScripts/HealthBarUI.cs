using UnityEngine;
using UnityEngine.UI; 

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] Slider healthSlider;

    void OnEnable()
    {
        if(health.CurrentHealth == 0)
        {
            Debug.Log("Player should be dead!");
        }

        health.OnHealthChange += UpdateBar;
        UpdateBar(health.CurrentHealth, health.MaxHealth);

    }

    void OnDisable()
    {
        health.OnHealthChange -= UpdateBar;
    }

    void UpdateBar(float currentHealth, float maxHealth)
    {
        healthSlider.value = maxHealth > 0f ? currentHealth / maxHealth : 0f;

    }

    void Start()
    {
        UpdateBar(health.CurrentHealth, health.MaxHealth);
    }
}
