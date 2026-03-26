using UnityEngine;

public class CleanEvent : MonoBehaviour
{
    private GameManager GM;

    public bool CleanDayEvent = false;
    public bool CleanDayEventFin = false;
    public bool CleanDayEvent_Clear = false;
    public bool CleanDayEvent_Fail = false;
    //제한 시간
    public float CleanDayEvent_TimeLimit = 120.0f;
    public int CleanDayEvent_Count = 0;

    void Start()
    {
        GM = FindAnyObjectByType<GameManager>();
        if (GM == null) Debug.LogError("[CleanEvent] GameManager를 찾을 수 없습니다.");
    }


    void Update()
    {
        if (CleanDayEvent && !CleanDayEventFin)
        {
            CleanDayEvent_TimeLimit -= Time.deltaTime;

            if (CleanDayEvent_TimeLimit <= 0f)
            {
                CheckTimeOutCleanEvent();
            }
        }
    }

    public void CheckTimeOutCleanEvent()
    {
        if (CleanDayEvent_Count >= 7)
        {
            Debug.Log("매우 우수 : 다음날 손님 카운트 2배");
            if (GM != null) GM.CustomerCountDoubleEvent = true;
        }
        else if (CleanDayEvent_Count < 7 && CleanDayEvent_Count >= 4)
        {
            Debug.Log("우수 : 손님 카운트 변화 없음");
        }
        else if (CleanDayEvent_Count < 4)
        {
            Debug.Log("보통 : 다음날 모든 빵 손님 카운트 1로 변경");
            if (GM != null) GM.CustomerCountPenaltyEvent = true;
        }

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
        }
        else
        {
            CleanDayEvent_Clear = true;
        }
    }
}
