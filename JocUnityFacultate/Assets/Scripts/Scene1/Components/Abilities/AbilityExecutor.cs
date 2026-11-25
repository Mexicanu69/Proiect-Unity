using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityExecutor : MonoBehaviour
{
    [SerializeField] AbilityData ability;
    [SerializeField] int maxTargets = 3;
    private float playerMana;

    float nextCastTime;
    readonly List<EnemyEntity> hits = new List<EnemyEntity>();

    private void Update()
    {
        playerMana = gameObject.GetComponent<Mana>().CurrentMana;

        if (Keyboard.current.eKey.wasPressedThisFrame && Time.time >= nextCastTime)
        {
            Cast(ability.manaCost);
            nextCastTime = Time.time + ability.cooldown;
        }
    }

    void Cast(float manaCost)
    { 

        switch (ability.targetMode)
        {
            case TargetMode.Self:
                Execute(gameObject);
                break;

            case TargetMode.AllInRange:
                foreach (var e in GetEnemiesInRange())
                    if (playerMana >= manaCost)
                    {
                        Execute(e.gameObject);
                    }
                    else
                    {
                        Debug.Log("Insuficient mana");
                    }
                break;
        }
    }

    public void Execute(GameObject target)
    {
        foreach (var effect in ability.effects)
            effect.Execute(gameObject, target, ability.manaCost);
    }

    List<EnemyEntity> GetEnemiesInRange()
    {
        hits.Clear();
        float rangeSqr = ability.range * ability.range;

        foreach (var e in EnemyEntity.Active)
        {
            if ((e.transform.position - transform.position).sqrMagnitude <= rangeSqr)
            {
                hits.Add(e);
                if (maxTargets > 0 && hits.Count >= maxTargets) break;
            }
        }
        return hits;
    }
}