using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum BreadType
{
    None,
    DollCake,
    MushroomMuffin,
    SlimePudding
}

public class DayEvent : MonoBehaviour
{
    private GameManager GM;

    [Header("낮 이벤트")]
    //낮 시간
    public int DayTime = 0;
    //손님 프리팹
    public GameObject CustomerPrefab;
    //손님 스폰 위치
    public Transform CustomerSpawnPoint;
    //손님 수
    public int Customer = 0;
    public int MaxCustomer = 3;

    //영업종료 -> 밤으로 전환
    public bool DayEventFin = false;

    [Header("빵 이벤트")]
    public bool BreadEvent = false;
    //빵 판매갯수
    public int BreadSellCount = 0;
    public int PerfectBread = 0;

    public bool CleanDayEvent = false;
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
        if (Customer >= MaxCustomer)
        {
            //밤으로 넘어가는 UI가 뜨고 버튼 누르면 StartNight실행
            DayEventUI dayEventUI = FindAnyObjectByType<DayEventUI>();
            if (dayEventUI == null) return;

            dayEventUI.DayFinUI.SetActive(true);

            DayEventFin = true;
        }

        if (!CleanDayEvent && GM.DayCount == 8)
        {
            StartCleanDayEvent();
            CleanDayEvent = true;
        }

        if (CleanDayEvent && !CleanDayEvent_Clear && !CleanDayEvent_Fail)
        {
            CleanDayEvent_TimeLimit -= Time.deltaTime;

            if (CleanDayEvent_TimeLimit <= 0f)
            {
                CheckTimeOutCleanEvent();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CustomerRandomOrder();
        }
    }

    public void CustomerRandomOrder()
    {
        if (GM == null) return;

        List<BreadType> SellableBreads = new List<BreadType>();

        if (GM.DollCake) SellableBreads.Add(BreadType.DollCake);
        if (GM.MushroomMuffin) SellableBreads.Add(BreadType.MushroomMuffin);
        if (GM.SlimePudding) SellableBreads.Add(BreadType.SlimePudding);

        if (SellableBreads.Count == 0)
        {
            Debug.LogWarning("판매 가능한 빵이 없습니다.");
            return;
        }

        int randomIndex = Random.Range(0, SellableBreads.Count);
        BreadType orderedBread = SellableBreads[randomIndex];

        bool isPackaging = (Random.Range(0, 2) == 1);

        Debug.Log($"주문한 빵 : {orderedBread}, 포장 : {isPackaging}");

        //아래에 손님 소환 or 주문UI 코드짜기
        GameObject newCustomer = Instantiate(CustomerPrefab, CustomerSpawnPoint.position, Quaternion.identity);
        Customer customer = newCustomer.GetComponent<Customer>();

        PackagingStation station = FindAnyObjectByType<PackagingStation>();
        DayEventUI dayEvnetUI = FindAnyObjectByType<DayEventUI>();
        if (dayEvnetUI == null || station == null) return;

        station.FindCustomer(customer);

        //주문UI 출력하기
        dayEvnetUI.OrderedBread(orderedBread, isPackaging);
        customer.SetOrder(orderedBread, isPackaging);
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
        SceneManager.LoadScene("CleanEventScene");
        Debug.Log("위생 점검 이벤트로 이동");
    }

    public void CheckTimeOutCleanEvent()
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

    public void ResetCustomer()
    {
        Customer = 0;
    }

    private void CustomerEvent()
    {
        Debug.Log($"손님 증가");
    }
}
