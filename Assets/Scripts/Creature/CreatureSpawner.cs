using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnCreatureType
{
    Doll,
    Mushroom,
    Horse
}

public class CreatureSpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    public SpawnCreatureType MyCreatureType;
    public GameObject CreaturePrefab;
    public Transform[] SpawnPoints;

    [Header("리스폰 조건")]
    public Transform PlayerTransform;
    public float RespawnDistance = 15f;

    [Header("정보")]
    public int MaxRespawnCount;

    private int CurrentRespawnCount = 0;


    void Start()
    {
        if (PlayerTransform == null) PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        int currentDay = GameManager.Instance != null ? GameManager.Instance.DayCount : 0;
        MaxRespawnCount = GetSpawnCountByDay(currentDay, MyCreatureType);

        if (MaxRespawnCount <= 0)
        {
            Debug.Log($"[CreatureSpawner] {currentDay}일차: {MyCreatureType} 스폰 없음 (비활성화)");
            gameObject.SetActive(false);
            return;
        }

        Debug.Log($"[CreatureSpawner] {currentDay}일차 - {MyCreatureType} 스폰 마릿수: {MaxRespawnCount}마리");


        StartCoroutine(SpawnAllCreaturesRoutine());
    }

    private IEnumerator SpawnAllCreaturesRoutine()
    {
        List<Transform> spawnablePoints = new List<Transform>(SpawnPoints);

        for (int i = 0; i < MaxRespawnCount; i++)
        {
            if (spawnablePoints.Count == 0)
            {
                spawnablePoints = new List<Transform>(SpawnPoints);
            }

            int randomIndex = Random.Range(0, spawnablePoints.Count);
            Transform targetPoint = spawnablePoints[randomIndex];

            spawnablePoints.RemoveAt(randomIndex);

            SpawnCreatureAt(targetPoint);

            yield return new WaitForSeconds(0.1f);
        }
    }

    private int GetSpawnCountByDay(int day, SpawnCreatureType type)
    {
        if (day == 8) return 0;
        if (day > 15) day = 15;

        int[] dollCounts = { 0, 10, 10, 10, 10, 12, 11, 9, 0, 8, 7, 8, 8, 6, 2, 1 };
        int[] mushroomCounts = { 0, 0, 0, 0, 0, 3, 4, 6, 0, 7, 8, 10, 8, 8, 8, 9 };
        int[] horseCounts = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 4, 6, 10, 10 };

        switch (type)
        {
            case SpawnCreatureType.Doll: return dollCounts[day];
            case SpawnCreatureType.Mushroom: return mushroomCounts[day];
            case SpawnCreatureType.Horse: return horseCounts[day];
        }
        return 0;
    }

    private void SpawnCreatureAt(Transform targetPoint)
    {
        CurrentRespawnCount++;

        GameObject spawnedCreature = Instantiate(CreaturePrefab, targetPoint.position, targetPoint.rotation);

        Creature creatureScript = spawnedCreature.GetComponent<Creature>();
        if (creatureScript != null)
        {
            creatureScript.SetupSpawnInfo(this, 0f);
        }

        if (MyCreatureType == SpawnCreatureType.Doll)
        {
            Creature_Doll dollScript = spawnedCreature.GetComponent<Creature_Doll>();
            if (dollScript != null)
            {
                List<Transform> itemPoints = new List<Transform>();
                foreach (Transform child in targetPoint)
                {
                    itemPoints.Add(child);
                }

                dollScript.SetItemSpawnLocations(itemPoints.ToArray());
            }
        }
        Debug.Log($"[CreatureSpawner] 크리쳐 스폰 완료 ({CurrentRespawnCount} / {MaxRespawnCount}), 위치: {targetPoint.name}");
    }
}
