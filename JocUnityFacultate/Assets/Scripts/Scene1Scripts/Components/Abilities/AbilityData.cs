using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="AbilityData", menuName ="ScriptableObjects/AbilityData")]

public class AbilityData : ScriptableObject
{
    public string label;
    [SerializeReference, SubclassSelector] public List<IEffect> effects;
    public float cooldown = 2f;
    public float manaCost = 10f;
    [SerializeReference, SubclassSelector] public ITargeting targeting = new SelfTargeting();

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(label)) label = name;
        if(effects == null) effects = new List<IEffect>();
    }
}