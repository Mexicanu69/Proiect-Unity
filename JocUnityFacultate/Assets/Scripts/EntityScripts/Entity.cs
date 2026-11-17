using UnityEngine;

[RequireComponent(typeof(Health))]
public abstract class Entity : MonoBehaviour
{
    public Health Health { get; private set; }

    protected virtual void Awake()
    {
        Health = GetComponent<Health>();
    }
}