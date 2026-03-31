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
    public float MinRespawnTime = 5f;
    public float MaxRespawnTime = 10f;

    [Header("패널티")]
    public int MaxRespawnCount;
    private int CurrentRespawnCount = 0;
    private int ConsecutiveEscapes = 0;
    private float AlertnessBonus = 0f;

    private GameObject SpawnedCreature;
    private bool isWaitingToRespawn = false;
    private Camera MainCamera;

    void Start()
    {
        if (SpawnArea == null) SpawnArea = GetComponent<BoxCollider>();
        if (PlayerTransform == null) PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        MainCamera = Camera.main;

        Creature creatureScript = CreaturePrefab.GetComponent<Creature>();
        if (creatureScript != null && creatureScript.creatureData != null)
        {
            if (GameManager.Instance != null)
            {
                if (creatureScript.creatureData.UnLockDay > GameManager.Instance.DayCount)
                {
                    Debug.Log($"[CreatureSpawner] {creatureScript.creatureData.name}은 아직 해금되지 않았습니다.");
                    gameObject.SetActive(false);
                    return;
                }
            }
        }
        else
        {
            Debug.LogError("[CreatureSpawner] 스폰할 프리팹에 Creature 스크립트나 데이터가 없습니다.");
            return;
        }
        MaxRespawnCount = Random.Range(2, 5);
        Debug.Log($"[CreatureSpawner] 최대 스폰 가능횟수 {MaxRespawnCount}");

        SpawnCreature();
    }

    void Update()
    {
        if (SpawnedCreature == null && !isWaitingToRespawn)
        {
            if (CurrentRespawnCount >= MaxRespawnCount)
            {
                Debug.Log($"[CreatureSpawner] {gameObject.name}의 스폰 횟수가 소진되었습니다.");
                gameObject.SetActive(false);
                return;
            }

            StartCoroutine(RespawnRoutine());
        }
    }

    private Vector3 GetRandomPosition()
    {
        Bounds bounds = SpawnArea.bounds;
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        Vector3 rayStartPos = new Vector3(randomX, bounds.max.y + 2f, randomZ);

        RaycastHit[] hits = Physics.RaycastAll(rayStartPos, Vector3.down, 100f);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == SpawnArea)
            {
                continue;
            }
            if (hit.collider.GetComponent<Creature>() != null)
            {
                continue;
            }
            return hit.point;
        }
        float fallbackY = bounds.min.y;

        return new Vector3(randomX, fallbackY, randomZ);
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

        Debug.Log($"[CreatureSpawner] 크리쳐 스폰 완료 ({CurrentRespawnCount} / {MaxRespawnCount}), {spawnPos}");
    }

    private IEnumerator RespawnRoutine()
    {
        isWaitingToRespawn = true;

        float waitTime = Random.Range(MinRespawnTime, MaxRespawnTime);

        if (ConsecutiveEscapes > 0)
        {
            float penaltyTime = ConsecutiveEscapes * 10f;
            waitTime += penaltyTime;
        }

        Debug.Log($"[CreatureSpawner] {waitTime:F1}초 뒤에 리스폰 됩니다.");

        yield return new WaitForSeconds(waitTime);

        yield return new WaitUntil(() => Vector3.Distance(transform.position, PlayerTransform.position) >= RespawnDistance || IsOutofCameraView());
        Debug.Log("[CreatureSpawner] 플레이어 시야 밖 확인, 스폰");

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
        if (MainCamera == null) return true;

        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);

        return viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1 || viewPos.z < 0;
    }
}
