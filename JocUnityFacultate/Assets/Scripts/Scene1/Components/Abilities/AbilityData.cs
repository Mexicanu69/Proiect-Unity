using System.Collections.Generic;
using UnityEngine;


public enum TargetMode { AllInRange, Self }

[CreateAssetMenu(fileName ="AbilityData", menuName ="ScriptableObjects/AbilityData")]

public class AbilityData : ScriptableObject
{
    public string label;
    public List<AbilityEffect> effects;
    public TargetMode targetMode;
    public float cooldown = 0.1f;
    public float range = 5f;
    public float manaCost = 10f;

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(label)) label = name;
        if(effects == null) effects = new List<AbilityEffect>();
    }
}