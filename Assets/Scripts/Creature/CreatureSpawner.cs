using System.Collections;
using UnityEngine;

public class CreatureSpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    public GameObject CreaturePrefab;
    public BoxCollider SpawnArea;

    [Header("리스폰 조건")]
    public Transform PlayerTransform;
    public float RespawnDistance = 15f;
    public float MinRespawnTime = 30f;
    public float MaxRespawnTime = 60f;

    [Header("패널티")]
    public int MaxRespawnCount;
    private int CurrentRespawnCount = 0;
    private int ConsecutiveEscapes = 0;
    private float AlertnessBonus = 0f;

    private GameObject SpawnedCreature;
    private bool isWaitingToRespawn = false;

    void Start()
    {
        if (SpawnArea == null) SpawnArea = GetComponent<BoxCollider>();
        if (PlayerTransform == null) PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        Creature creatureScript = CreaturePrefab.GetComponent<Creature>();
        if (creatureScript != null && creatureScript.creatureData != null)
        {
            GameManager GM = FindAnyObjectByType<GameManager>();
            if (GM != null)
            {
                if (creatureScript.creatureData.UnLockDay > GM.DayCount)
                {
                    gameObject.SetActive(false);
                    return;
                }
            }
        }
        else
        {
            Debug.LogError("스폰할 프리팹에 Creature 스크립트나 데이터가 없습니다.");
            return;
        }
        MaxRespawnCount = Random.Range(1, 4);
        Debug.Log($"최대 스폰 가능횟수 {MaxRespawnCount}");

        SpawnCreature();
    }

    void Update()
    {
        if (SpawnedCreature == null && !isWaitingToRespawn)
        {
            if (CurrentRespawnCount >= MaxRespawnCount)
            {
                gameObject.SetActive(false);
                return;
            }
            if (Vector3.Distance(transform.position, PlayerTransform.position) >= RespawnDistance || IsOutofCameraView())
            {
                Debug.Log("리스폰 조건 만족");

                StartCoroutine(RespawnRoutine());
            }
        }
    }

    private Vector3 GetRandomPosition()
    {
        Bounds bounds = SpawnArea.bounds;
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(randomX, randomY, randomZ);
    }

    private void SpawnCreature()
    {
        CurrentRespawnCount++;
        Vector3 spawnPos = GetRandomPosition();
        SpawnedCreature = Instantiate(CreaturePrefab, spawnPos, Quaternion.identity);

        Creature creatureScript = SpawnedCreature.GetComponent<Creature>();
        if (creatureScript != null)
        {
            creatureScript.SetupSpawnInfo(this, AlertnessBonus);
        }

        Debug.Log($"크리쳐 스폰 완료 : {spawnPos}");
    }

    private IEnumerator RespawnRoutine()
    {
        isWaitingToRespawn = true;
        float waitTime = Random.Range(MinRespawnTime, MaxRespawnTime);

        if (ConsecutiveEscapes > 0)
        {
            float penaltyTime = ConsecutiveEscapes * 60f;
            waitTime += penaltyTime;
        }
        Debug.Log($"{waitTime:F1}초 뒤에 리스폰 됩니다.");

        yield return new WaitForSeconds(waitTime);

        SpawnCreature();
        isWaitingToRespawn = false;
    }

    public void ReportEscape()
    {
        ConsecutiveEscapes++;
        AlertnessBonus += 0.1f;
    }

    public void ReportCapture()
    {
        ConsecutiveEscapes = 0;
    }

    private bool IsOutofCameraView()
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);

        return viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1 || viewPos.z < 0;
    }
}
