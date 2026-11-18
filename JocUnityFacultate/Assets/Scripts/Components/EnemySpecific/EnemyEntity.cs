using System;
using UnityEngine;

public class EnemyEntity : Entity
{
    public EnemyCreation Creation { get; private set; }

    // actiune de intoarcere in pool
    private Action<EnemyEntity> returnToPool;

    protected override void Awake()
    {
        base.Awake();
        Creation = GetComponent<EnemyCreation>();
        Health.OnDeath += HandleDeath;
    }

    public void SetReturnToPool(Action<EnemyEntity> action)
    {
        returnToPool = action;
    }

    private void HandleDeath()
    {
        if (returnToPool != null)
            returnToPool(this);
        else
            // l-am pus eu manual daca nu e din object pool
            // deci pot sa il Destroy()
            Destroy(gameObject);
    }
}
