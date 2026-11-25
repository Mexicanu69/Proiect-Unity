using UnityEngine;
using UnityEngine.UI; 

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] Slider healthSlider;

    void OnEnable() => health.OnDamaged += UpdateBar;

    void OnDisable()
    {
        health.OnDamaged -= UpdateBar;
    }

    void UpdateBar(float _)
    {
        if(health.MaxHealth==0)
        {
            Debug.Log($"Max health egal cu 0");
            return;
        }
        float sliderSize = 1f - (health.CurrentHealth / health.MaxHealth);
        //asociem cu UI ul    
        healthSlider.value = sliderSize;

        }


    void Start()
    {
        UpdateBar(0);
    }
}
