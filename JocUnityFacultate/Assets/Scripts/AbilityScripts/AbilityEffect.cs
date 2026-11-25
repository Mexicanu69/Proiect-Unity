using System;
using UnityEngine;

[Serializable]
public abstract class AbilityEffect : ScriptableObject
{
    public abstract void Execute(GameObject caster, GameObject target, float manaCost);
}

[CreateAssetMenu(menuName = "Effects/Damage")]
public class DamageEffect : AbilityEffect
{
    public int amount;
    public override void Execute(GameObject caster, GameObject target, float manaCost)
    {
        target.GetComponent<Health>().TakeDamage(amount);
        caster.GetComponent<Mana>().ConsumeMana(manaCost);
        Debug.Log($"{caster.name} dealt {amount} damage to {target.name}");
    }
}