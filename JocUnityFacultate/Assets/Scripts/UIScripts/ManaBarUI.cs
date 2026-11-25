using UnityEngine;
using UnityEngine.UI;

public class ManaBarUI : MonoBehaviour
{
    [SerializeField] Mana mana;
    [SerializeField] Slider manaSlider;

    void OnEnable()
    {
        if (mana == null) return;
        mana.OnManaChanged += UpdateBar;
        UpdateBar(mana.CurrentMana, mana.MaxMana);
    }

    void OnDisable()
    {
        if (mana == null) return;
        mana.OnManaChanged -= UpdateBar;
    }

    void UpdateBar(float current, float max)
    {
        manaSlider.value = max > 0f 
            ? current / max 
            : 0f;
    }
}