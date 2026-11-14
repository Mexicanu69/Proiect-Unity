using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Enemies/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Info")]
    public string displayName = "Enemy";

    [Header("Health")]
    public int maxHealth = 50;
    public int currentHealth = 50;

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("AI")]
    public float detectionRange = 8f;

    [Header("Attack")]
    public int damage = 10;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;

    [Header("Rewards")]
    public int xpReward = 20;
}