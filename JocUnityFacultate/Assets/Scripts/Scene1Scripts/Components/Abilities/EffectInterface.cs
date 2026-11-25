using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * pentru logica de baza am urmarit tutorialul: https://www.youtube.com/watch?v=L0cXb-ettK4
 * Combin o serie de interfate + clase pentru a face un sistem de abilitati mai complex
 * Fiecare abilitate are o serie de efecte.
 * Clasa efect este o clasa ce implementeaza o interfata de tip IEfect cu restrictia ca T sa fie clasa/ interfata referinta spre o clasa
 * Ne intereseaza sa avem interfete pentru tipurile de efecte pentru ca de exemplu pot lua damage nu numai inamicii, ci si obiecte in jur
 * Poate daca adaugam niste caractere care sa ajute playerul sa supravietuiasca, acestea sa fie si ele mana users. 
 *
 */

public interface IDamageable
{ 
    public void TakeDamage(float amount);
    public void GainHealth(float amount);
}

public interface IManaUser
{
    float MaxMana{  get; }
    float CurrentMana { get; }
    public void RestoreMana(float amount);
    public void ConsumeMana(float amount);
}

public interface ITargeting
{
    void GetTargets(GameObject caster, List<GameObject> results);
}
public interface IEffect
{
    void Apply(GameObject caster, GameObject target);
    void Cancel();
    //event care asculta daca efectul s-a completat (i.e poison 3 secunde, poate vrem sa dispara animatia sau sa se intample ceva dupa completion)
    event Action<IEffect, GameObject> OnCompleted;
}

[Serializable]
public abstract class Effect<T> : IEffect where T : class
{
    public event Action<IEffect, GameObject> OnCompleted;
    public void Apply(GameObject caster, GameObject target)
    {
        //luam componenta care implementeaza tipul de interfata T (adica IDamageable, IManaUser, etc)
        if(target.TryGetComponent(out T typedTarget))
        {
            //incercam sa aplicam acest efect pe target
            ApplyTo(caster, typedTarget, target);
        }
        else
        {
            //daca nu avem componenta pe target atunci semnalam efectul ca fiind completat
            Complete(target);
        }
    }

    //fiecare tip de apply tine de efect in sine
    protected abstract void ApplyTo(GameObject caster, T target, GameObject targetObject);
    public virtual void Cancel() { }
    //trimitem mai departe efectul care s-a completat si targetul
    protected void Complete(GameObject targetObject) => OnCompleted?.Invoke(this, targetObject);
}

[Serializable]
public class DamageEffect : Effect<IDamageable>
{
    public int damageAmount = 10;
    protected override void ApplyTo(GameObject caster, IDamageable target, GameObject obj)
    {
        target.TakeDamage(damageAmount);
        Complete(obj);
    }
}



[Serializable]

public class HealthRegen : Effect<IDamageable>
{
    public int healthRegenAmount = 10;
    protected override void ApplyTo(GameObject caster, IDamageable target, GameObject obj)
    {
        target.GainHealth(healthRegenAmount);
        Complete(obj);
    }
}

[Serializable]
public class ManaOverTime : Effect<IManaUser>
{
    [Range(0, 1f)] public float percentOfMaxMana = 0.3f; //cat % din mana sa se regenereze
    public float duration = 5f; //in cat timp

    [NonSerialized] Coroutine routine;
    [NonSerialized] MonoBehaviour host;

    protected override void ApplyTo(GameObject caster, IManaUser target, GameObject obj)
    {
        MonoBehaviour mb = target as MonoBehaviour;
        if (mb == null || !mb.isActiveAndEnabled)
        {
            Complete(obj);
            return;
        }
        
        //daca in timpul routinei se apasa iarasi  abilitatea o restartam
        if (host != null && routine != null)
        {
            host.StopCoroutine(routine);
        }

        host = mb;
        float expectedManaVal = target.MaxMana * percentOfMaxMana;
        routine = mb.StartCoroutine(Regen(target, obj, expectedManaVal));
    }

    IEnumerator Regen(IManaUser target, GameObject obj, float total)
    {
        float restored = 0;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float dt = Time.deltaTime;
            elapsed += dt;

            float step = Mathf.Min(total * dt / duration, total - restored);
            target.RestoreMana(step);
            restored += step;

            yield return null;
        }

        routine = null;
        Complete(obj);
    }

    public override void Cancel()
    {
        if(host != null && routine != null)
        {
            host.StopCoroutine(routine);
        }

        routine = null;
    }
}

[Serializable]
public class SelfTargeting : ITargeting
{
    public void GetTargets(GameObject caster, List<GameObject> targets)
    {
        targets.Add(caster);
    }
}

[Serializable]
public class AreaTargeting: ITargeting
{
    public float range = 5f;
    public int maxTargets = 0;
    public void GetTargets(GameObject caster, List <GameObject> targets)
    {
        float rangeSqr = range * range;
        Vector3 origin = caster.transform.position;

        foreach (var enemy in EnemyEntity.Active)
        {
            Vector3 enemyPos = enemy.transform.position;
            if ((enemyPos - origin).sqrMagnitude < rangeSqr)
            {
                targets.Add(enemy.gameObject);
                if (maxTargets > 0 && targets.Count >= maxTargets) break;
            }
        }
    }
}