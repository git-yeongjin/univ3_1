using System.Collections.Generic;
using UnityEngine;

public class CleanEvent : MonoBehaviour
{
    private GameManager GM;
    public CleanDayUI cleanDayUI;

    public bool CleanDayEvent = false;
    public bool CleanDayEventFin = false;
    public bool CleanDayEvent_Clear = false;
    public bool CleanDayEvent_Fail = false;
    //제한 시간
    public float CleanDayEvent_TimeLimit = 120.0f;

    public int CleanDayEvent_Count = 0;
    public int RemainingCreatureTraces = 3;

    [Header("스폰 설정")]
    public GameObject NormalTrashPrefab;
    public GameObject CreatureTracePrefab;
    public Transform TracePointRoot;
    public CleanEventNPC InspectorNPC;  

    private List<Transform> SpawnPoints = new List<Transform>();


    void Start()
    {
        GM = FindAnyObjectByType<GameManager>();
        if (GM == null) Debug.LogError("[CleanEvent] GameManager를 찾을 수 없습니다.");

        if (TracePointRoot != null)
        {
            //Point_1,2 돌기
            foreach (Transform pointGroup in TracePointRoot)
            {
                //CleanPoint_1, 2, 3 돌기
                foreach (Transform cleanPoint in pointGroup)
                {
                    SpawnPoints.Add(cleanPoint);
                }
            }
            Debug.Log($"[CleanEvent] 총 {SpawnPoints.Count}개의 청소 스폰 포인트를 찾았습니다");
        }
    }

    void Update()
    {
        if (CleanDayEvent && !CleanDayEventFin)
        {
            CleanDayEvent_TimeLimit -= Time.deltaTime;

            if (CleanDayEvent_TimeLimit <= 0f)
            {
                CleanDayEventFin = true;

                if (InspectorNPC != null)
                {
                    InspectorNPC.AppearAndReady();
                }
                else
                {
                    CheckTimeOutCleanEvent(); // NPC가 없으면 그냥 바로 정산
                }
            }
        }
    }

    public void StartCleanEvent()
    {
        CleanDayEvent = true;
        CleanDayEventFin = false;
        CleanDayEvent_Clear = false;
        CleanDayEvent_Fail = false;
        CleanDayEvent_TimeLimit = 120f;

        CleanDayEvent_Count = 0;
        RemainingCreatureTraces = 3;

        SpawnTrashes();
    }

    private void SpawnTrashes()
    {
        if (SpawnPoints.Count < 10)
        {
            Debug.LogWarning("[CleanEvent] 스폰 포인트가 10개 미만입니다)");
        }

        List<Transform> availablePoints = new List<Transform>(SpawnPoints);

        //일반 쓰레기 7개 랜덤 스폰
        for (int i = 0; i < 7; i++)
        {
            int randomIndex = Random.Range(0, availablePoints.Count);
            Transform targetPoint = availablePoints[randomIndex];

            Instantiate(NormalTrashPrefab, targetPoint.position, targetPoint.rotation, targetPoint);
            availablePoints.RemoveAt(randomIndex);
        }

        //남은 자리에 크리쳐 흔적 3개 랜덤 스폰
        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, availablePoints.Count);
            Transform targetPoint = availablePoints[randomIndex];

            Instantiate(CreatureTracePrefab, targetPoint.position, targetPoint.rotation, targetPoint);
            availablePoints.RemoveAt(randomIndex);
        }

        Debug.Log("[CleanEvent] 일반 쓰레기 7개, 크리쳐 흔적 3개 랜덤 스폰 완료");
    }

    public void CheckTimeOutCleanEvent()
    {
        CleanDayEventFin = true;

        int resultLevel = 0;
        if (CleanDayEvent_Count >= 7)
        {
            Debug.Log("매우 우수 : 다음날 손님 카운트 2배");
            if (GM != null) GM.CustomerCountDoubleEvent = true;
            resultLevel = 0;
        }
        else if (CleanDayEvent_Count < 7 && CleanDayEvent_Count >= 4)
        {
            Debug.Log("우수 : 손님 카운트 변화 없음");
            resultLevel = 1;
        }
        else if (CleanDayEvent_Count < 4)
        {
            Debug.Log("보통 : 다음날 모든 빵 손님 카운트 1로 변경");
            if (GM != null) GM.CustomerCountPenaltyEvent = true;
            resultLevel = 2;
        }

        // CleanDayUI를 통해 결과창 팝업
        if (cleanDayUI != null) cleanDayUI.ShowResultUI(resultLevel);
        CleanDayEventFin = true;
    }

    public void IncreaseCount()
    {
        CleanDayEvent_Count++;
    }

    public void CheckCreatureTrace(bool trace)
    {
        if (trace)
        {
            Debug.Log("위생 검사에서 크리쳐 흔적이 발견되어 게임 끝");
            CleanDayEvent_Fail = true;

            if (cleanDayUI != null)
            {
                cleanDayUI.TriggerCleanEnding();
            }
        }
        else
        {
            CleanDayEvent_Clear = true;
        }
    }
}
