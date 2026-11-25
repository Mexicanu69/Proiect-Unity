using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Enemies/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Identity")]
    public string enemyName;
    public GameObject prefab;

    [Header("Health")]
    [Min(1)] public float maxHealth = 20f;

    [Header("Movement")]
    [Min(0)] public float moveSpeed = 3.5f;

    [Header("Auto-Attack")]
    [Min(0)] public float attackDamage = 5f;
    [Min(0)] public float attackRange = 1.5f;
    [Min(0)] public float attackCooldown = 1f;

    [Header("Loot")]
    [Min(0)] public int xpReward = 1;

    [Header("Spawn Behavior")]
    [Min(1)] public int cost = 10;
    [Min(0)] public float spawnWeight = 1f;
    [Min(0)] public float minGameTime = 0f;
}
