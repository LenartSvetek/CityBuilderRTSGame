using NaughtyAttributes;
using UnityEngine;

public class HordeSpawner : MonoBehaviour
{
    [SerializeField]
    Transform OutpostTransform;

    [SerializeField]
    GameObject EnemyPrefab;

    [SerializeField]
    [Tooltip("in ms")]
    float hordeSpawnTimeout = 60 * 1000f;
    [SerializeField]
    [ReadOnly]
    float hordeSpawnTimer = 0f;
    [SerializeField]
    float radius = 100f;


    [SerializeField]
    float hordeUnitsAmount = 3;

    [SerializeField]
    float hordeUnitsRoundMultiplier = 1.1f;
    [SerializeField]
    int hordeUnitsIncreaseAmountMax = 20;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hordeSpawnTimer = hordeSpawnTimeout;
    }

    // Update is called once per frame
    void Update()
    {
        hordeSpawnTimer -= Time.unscaledDeltaTime * 1000f;
        if(hordeSpawnTimer < 0)
        {
            hordeSpawnTimer = hordeSpawnTimeout;
            SpawnHorde();
        }
    }

    [Button("Spawn horde")]
    void SpawnHordeManualy()
    {
        hordeSpawnTimer = hordeSpawnTimeout;
        SpawnHorde();
    }
    
    void SpawnHorde()
    {
        float angleStep = 360f / hordeUnitsAmount;
        for (int i = 0; i < hordeUnitsAmount; i++)
        {
            float currentAngle = i * angleStep;
            float radians = currentAngle * Mathf.Deg2Rad;

            float x = OutpostTransform.position.x + Mathf.Cos(radians) * radius;
            float z = OutpostTransform.position.z + Mathf.Sin(radians) * radius;

            Vector3 checkPosition = new Vector3(x, 0, z);

            float y = Terrain.activeTerrain.SampleHeight(checkPosition);
            Vector3 pos = new Vector3(x, y, z);

            var enemy = Instantiate(EnemyPrefab);
            enemy.transform.position = pos;

            var warrior = enemy.GetComponent<WarriorComp>();
            warrior.enabled = true;

            warrior.SetTarget(OutpostTransform);
        }

        hordeUnitsAmount = Mathf.Min(hordeUnitsAmount + hordeUnitsIncreaseAmountMax, hordeUnitsAmount * hordeUnitsRoundMultiplier);

        return;
    }
}
