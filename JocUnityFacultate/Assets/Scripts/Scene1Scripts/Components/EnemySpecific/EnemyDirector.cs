using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemyDirector : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField] private Transform target;

    [Header("Enemies")]
    [SerializeField] private List<EnemyData> enemyList = new List<EnemyData>();

    [Header("Points")]
    [SerializeField, Min(0)] private float startingPoints = 20f;
    [SerializeField, Min(0)] private float pointsPerSecond = 2f;
    [SerializeField, Min(0)] private float difficultyPerMinute = 0.5f;

    [Header("Spawning")]
    [SerializeField, Min(0.1f)] private float spawnInterval = 3f;
    [SerializeField, Min(1)] private int maxSpawnsPerWave = 8;
    [SerializeField, Min(1)] private int maxAliveEnemies = 150;
    [SerializeField, Min(0)] private float minSpawnDistance = 18f;
    [SerializeField, Min(0)] private float maxSpawnDistance = 25f;

    [Header("Pooling")]
    [Tooltip("Cati inamici de fiecare tip incap in pool de la inceput")]
    [SerializeField, Min(1)] private int poolDefaultCapacity = 20;
    [Tooltip("Peste atatia inamici inactivi de un tip, cei in plus sunt distrusi")]
    [SerializeField, Min(1)] private int poolMaxSize = 200;

    [Header("Spawn Radius")]
    [SerializeField] private bool showSpawnRings = true;
    [SerializeField] private bool onlyWhenSelected = true;
    [SerializeField] private Color minRingColor = Color.yellow;
    [SerializeField] private Color maxRingColor = Color.red;
    [SerializeField, Range(1f, 10f)] private float ringThickness = 2f;

    [Header("Debug")]
    [SerializeField, ReadOnly] private float points;
    [SerializeField, ReadOnly] private float difficulty = 1f;
    [SerializeField, ReadOnly] private int aliveEnemies;
    [SerializeField, ReadOnly] private EnemyData nextEnemy;
    [SerializeField, ReadOnly] private int inactiveInPools;

    private float elapsedTime;
    private float spawnTimer;

    // fiecare enemy type are un pool
    private readonly Dictionary<EnemyData, ObjectPool<EnemyEntity>> pools =
        new Dictionary<EnemyData, ObjectPool<EnemyEntity>>();

    private Vector3 pendingSpawnPosition;

    void Start()
    {
        points = startingPoints;
    }

    void Update()
    {
        inactiveInPools = CountInactive();

        if (target == null) 
            return;

        elapsedTime += Time.deltaTime;
        difficulty = 1f + difficultyPerMinute * (elapsedTime / 60f);
        points += pointsPerSecond * difficulty * Time.deltaTime;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer > 0f) 
            return;
        spawnTimer = spawnInterval;

        SpawnWave();
    }

    private void SpawnWave()
    {
        if (nextEnemy == null)
            nextEnemy = PickEnemy();

        if (nextEnemy == null)
            return;              
        // economiseste puncte pt urm inamic
        if (points < nextEnemy.cost) 
            return;  

        int spawned = 0;

        while (points >= nextEnemy.cost && spawned < maxSpawnsPerWave && aliveEnemies < maxAliveEnemies)
        {
            if (!TrySpawn(nextEnemy)) 
                break;

            points -= nextEnemy.cost;
            spawned++;
        }

        nextEnemy = null;
    }

    private EnemyData PickEnemy()
    {
        float totalWeight = 0f;

        foreach (EnemyData enemy in enemyList)
            if (IsAvailable(enemy)) totalWeight += enemy.spawnWeight;

        if (totalWeight <= 0f) return null;

        float roll = Random.Range(0f, totalWeight);

        foreach (EnemyData enemy in enemyList)
        {
            if (!IsAvailable(enemy)) continue;
            roll -= enemy.spawnWeight;
            if (roll <= 0f) return enemy;
        }
        return null;
    }

    private bool IsAvailable(EnemyData enemy)
    {
        return enemy != null
            && enemy.prefab != null
            && enemy.spawnWeight > 0f
            && elapsedTime >= enemy.minGameTime;
    }

    private bool TrySpawn(EnemyData data)
    {
        if (!TryGetSpawnPosition(out Vector3 position)) 
            return false;

        pendingSpawnPosition = position;

        EnemyEntity enemy = GetPool(data).Get();

        if (enemy == null)
            return false;

        enemy.Creation.PlaceAt(position);
        enemy.Creation.Setup(data, target);

        aliveEnemies++;
        return true;
    }

    private ObjectPool<EnemyEntity> GetPool(EnemyData data)
    {
        if (pools.TryGetValue(data, out ObjectPool<EnemyEntity> pool))
            return pool;

        pool = new ObjectPool<EnemyEntity>(
            createFunc: () => CreateEnemy(data),
            actionOnGet: enemy => enemy.gameObject.SetActive(true),
            actionOnRelease: enemy => enemy.gameObject.SetActive(false),
            actionOnDestroy: enemy => Destroy(enemy.gameObject),
            collectionCheck: true,
            defaultCapacity: poolDefaultCapacity,
            maxSize: poolMaxSize);

        pools.Add(data, pool);
        return pool;
    }

    // fa un inamic nou cand pool e gol
    private EnemyEntity CreateEnemy(EnemyData data)
    {
        GameObject obj = Instantiate(data.prefab, pendingSpawnPosition, Quaternion.identity, transform);
        EnemyEntity enemy = obj.GetComponent<EnemyEntity>();

        if (enemy == null || enemy.Creation == null)
        {
            Debug.LogError($"{data.name} nu are EnemyEntity si EnemyCreation in prefab", data.prefab);
            Destroy(obj);
            return null;
        }

        // cand e creat, se aboneaza la on death
        // nu la spawn pentru ca daca se aboneaza la spawn si moare de mai multe ori scade mai mult numarul
        enemy.Health.OnDeath += () => aliveEnemies--;
        enemy.SetReturnToPool(deadEnemy => pools[data].Release(deadEnemy));

        return enemy;
    }

    private int CountInactive()
    {
        int total = 0;
        foreach (ObjectPool<EnemyEntity> pool in pools.Values)
            total += pool.CountInactive;
        return total;
    }

    private bool TryGetSpawnPosition(out Vector3 position)
    {
        Vector3 center = target.position;
        if (NavMesh.SamplePosition(center, out NavMeshHit ground, 10f, NavMesh.AllAreas))
            center = ground.position;

        for (int attempt = 0; attempt < 10; attempt++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float distance = Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * distance;

            if (NavMesh.SamplePosition(center + offset, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            {
                position = hit.position;
                return true;
            }
        }

        position = Vector3.zero;
        return false;
    }

    void OnValidate()
    {
        if (maxSpawnDistance < minSpawnDistance)
            maxSpawnDistance = minSpawnDistance;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!onlyWhenSelected)
            DrawSpawnRings();
    }

    void OnDrawGizmosSelected()
    {
        if (onlyWhenSelected)
            DrawSpawnRings();
    }

    private void DrawSpawnRings()
    {
        if (!showSpawnRings)
            return;

        // desenam in jurul playerului inelele de spawn
        Vector3 center = target != null ? target.position : transform.position;

        Handles.color = minRingColor;
        Handles.DrawWireDisc(center, Vector3.up, minSpawnDistance, ringThickness);

        Handles.color = maxRingColor;
        Handles.DrawWireDisc(center, Vector3.up, maxSpawnDistance, ringThickness);
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemyDirector))]
public class EnemyDirectorEditor : Editor
{
    void OnSceneGUI()
    {
        serializedObject.Update();

        if (!serializedObject.FindProperty("showSpawnRings").boolValue)
            return;

        SerializedProperty targetProp = serializedObject.FindProperty("target");
        SerializedProperty minProp = serializedObject.FindProperty("minSpawnDistance");
        SerializedProperty maxProp = serializedObject.FindProperty("maxSpawnDistance");

        Color minColor = serializedObject.FindProperty("minRingColor").colorValue;
        Color maxColor = serializedObject.FindProperty("maxRingColor").colorValue;

        EnemyDirector director = (EnemyDirector)target;
        Transform ringTarget = targetProp.objectReferenceValue as Transform;
        Vector3 center = ringTarget != null ? ringTarget.position : director.transform.position;

        float newMin = DragRadius(center, minProp.floatValue, minColor, "Min");
        float newMax = DragRadius(center, maxProp.floatValue, maxColor, "Max");

        if (!Mathf.Approximately(newMin, minProp.floatValue) ||
            !Mathf.Approximately(newMax, maxProp.floatValue))
        {
            minProp.floatValue = newMin;
            maxProp.floatValue = Mathf.Max(newMax, newMin);
            serializedObject.ApplyModifiedProperties();
        }
    }

    private static float DragRadius(Vector3 center, float radius, Color color, string label)
    {
        Vector3 handlePos = center + Vector3.right * radius;
        float size = HandleUtility.GetHandleSize(handlePos) * 0.12f;

        Handles.color = color;
        Handles.Label(handlePos + Vector3.up * size * 3f, $"{label}: {radius:F1}");

        EditorGUI.BeginChangeCheck();
        Vector3 newPos = Handles.Slider(handlePos, Vector3.right, size, Handles.DotHandleCap, 0f);

        if (EditorGUI.EndChangeCheck())
            return Mathf.Max(0f, newPos.x - center.x);

        return radius;
    }
}
#endif
