using UnityEngine;

public class DayEvent : MonoBehaviour
{
    private GameManager GM;

    [Header("낮 이벤트")]
    //낮 시간
    public int DayTime = 0;
    //손님 수
    public int Customer = 0;
    //수익
    public int Money = 0;
    //영업종료 -> 밤으로 전환
    public bool DayEventFin = false;

    [Header("빵 이벤트")]
    public bool BreadEvent = false;

    //퍼펙트 빵 갯수 5당 손님 증가
    public int PerfectBread = 0;
    //빵 판매갯수
    public int BreadSellCount = 0;

    [Header("위생점검 이벤트")]
    //8일차에 활성화
    public bool CleanDayEvent = false;
    //이벤트 통과 확인
    public bool CleanDayEvent_Clear = false;
    public bool CleanDayEvent_Fail = false;
    //제한 시간
    public float CleanDayEvent_TimeLimit = 120.0f;
    public int CleanDayEvent_Count = 0;

    void Start()
    {
        GM = gameObject.GetComponent<GameManager>();
        if (GM == null)
        {
            Debug.LogError($"GM을 못찾음");
        }
    }

    void Update()
    {
        if (DayEventFin)
        {
            //밤으로 넘어가는 UI가 뜨고 버튼 누르면 StartNight실행
        }
        if (!CleanDayEvent && GM.DayCount == 8)
        {
            StartCleanDayEvent();
        }
    }

    public void StartNight()
    {
        //밤 씬으로 이동하기
        //이동했으니 다시 false
        DayEventFin = false;
    }

    public void AddPerfectBread()
    {
        PerfectBread++;
        Debug.Log($"현재 완벽한 빵 개수 : {PerfectBread}");
        if (PerfectBread >= 5)
        {
            CustomerEvent();
        }
    }

    public void StartCleanDayEvent()
    {
        //Debug.Log("위생 관리 이벤트 발생");
        if (!CleanDayEvent_Clear && !CleanDayEvent_Fail)
        {
            CleanDayEvent_TimeLimit -= Time.deltaTime;

            //Debug.Log($"위생 관리 남은시간 : {CleanDayEvent_TimeLimit:F2}");
        }
        if (CleanDayEvent_TimeLimit <= 0f)
        {
            if (CleanDayEvent_Clear)
            {
                if (CleanDayEvent_Count == 7)
                {
                    Debug.Log("매우 우수");
                    CustomerEvent();
                }
                else if (CleanDayEvent_Count < 7 && CleanDayEvent_Count >= 4)
                {
                    Debug.Log("우수");
                }
                else if (CleanDayEvent_Count < 4)
                {
                    Debug.Log("좋음");
                }
            }
        }
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
        }
        else
        {
            CleanDayEvent_Clear = true;
        }
    }

    private void CustomerEvent()
    {
        Debug.Log($"손님 증가");
    }
}
