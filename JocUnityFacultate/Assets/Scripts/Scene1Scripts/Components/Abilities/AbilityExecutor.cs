using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Mana))]
public class AbilityExecutor : MonoBehaviour
{
    [SerializeField] AbilityData ability;
    Mana mana;
    float nextCastTime;
    readonly List<GameObject> targets = new List<GameObject>();


    private void Awake() => mana = GetComponent<Mana>();


    private void Update()
    {   //TODO: use new input system (this is just for test)
        if (Keyboard.current.eKey.wasPressedThisFrame && Time.time >= nextCastTime)
        {
            //daca exista componenta mana pe player dar nu mai are mana atunci rip
            if (mana != null && mana.CurrentMana < ability.manaCost)
            {
                Debug.Log("Insufficient mana");
                return;
            }

            //altfel consumam mana cu costul abilitatii, castam abilitatea, si punem cooldownul pentru abilitate la timpul urmatorului cast permis
            mana.ConsumeMana(ability.manaCost);
            Cast();
            nextCastTime = Time.time + ability.cooldown;
        }
    }

    void Cast()
    {
        targets.Clear();
        ability.targeting?.GetTargets(gameObject, targets);

        foreach(var target in targets)
        {
            Execute(target);
        }
    }

    //fiecare abilitate este executata apeland Apply()-ul fiecarui effect
    public void Execute(GameObject target)
    {
        foreach (var effect in ability.effects)
            effect.Apply(gameObject, target);
    }
}
    