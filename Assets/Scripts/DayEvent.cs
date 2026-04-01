using System.Collections.Generic;
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
    [Header("낮 시간 및 스폰 설정")]
    //낮 시간
    public int DayTime = 0;
    //손님 프리팹
    public GameObject CustomerPrefab;
    //손님 스폰 위치
    public Transform CustomerSpawnPoint;

    [Header("손님 및 영업 정보")]
    //현재까지 스폰 된 손님
    public int ActualCustomer = 0;
    //판매 완료한 손님
    public int ProcessedCustomer = 0;
    //총 손님 수
    public int MaxCustomer = 3;
    //손님 카운트
    public int CustomerScore = 0;
    //영업종료 -> 밤으로 전환
    public bool DayEventFin = false;

    public bool isCustomerPresent = false;

    [Header("빵 이벤트")]
    public bool BreadEvent = false;
    //빵 판매갯수
    public int BreadSellCount = 0;
    public int PerfectBread = 0;

    //무한 씬 로딩 방지
    private bool hasTriggeredCleanEvent = false;

    void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError($"[DayEvent] GameManager.Instance가 존재하지 않습니다.");
        }

        ResetDayEvent();
    }

    void Update()
    {
        if (GameManager.Instance.DayCount == 8 && !hasTriggeredCleanEvent)
        {
            hasTriggeredCleanEvent = true;
            StartCleanDayEvent();
        }

        //테스트 용
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CustomerRandomOrder();
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "DayEventScene")
        {
            Debug.Log($"[DayEvent] {scene.name} 씬,  낮 영업 세팅을 초기화합니다.");
            ResetDayEvent();
        }
    }

    public void ResetDayEvent()
    {
        ActualCustomer = 0;
        ProcessedCustomer = 0;
        CustomerScore = 0;
        DayEventFin = false;
        hasTriggeredCleanEvent = false;
        isCustomerPresent = false;

        if (GameManager.Instance.DayCount == 0)
        {
            MaxCustomer = 1;
            GameManager.Instance.DollCakeCount = 1;
        }
        else
        {
            MaxCustomer = Mathf.Min(GameManager.Instance.CustomerToCreature, 10);
        }
        if (MaxCustomer <= 0)
        {
            MaxCustomer = 1;
        }

        Debug.Log($"[DayEvnet] 오늘 예정된 손님 {MaxCustomer}명");
    }

    public void CustomerRandomOrder()
    {
        if (isCustomerPresent)
        {
            Debug.Log($"[DayEvent] 아직 손님이 있습니다.");
            return;
        }

        if (DayEventFin || ActualCustomer > MaxCustomer)
        {
            Debug.Log("[DayEvent] 오늘 모든 손님이 방문했습니다.");
            return;
        }

        List<BreadType> SellableBreads = new List<BreadType>();

        if (GameManager.Instance.DollCakeCount > 0) SellableBreads.Add(BreadType.DollCake);
        if (GameManager.Instance.MushroomMuffinCount > 0) SellableBreads.Add(BreadType.MushroomMuffin);
        if (GameManager.Instance.SlimePuddingCount > 0) SellableBreads.Add(BreadType.SlimePudding);

        if (SellableBreads.Count == 0)
        {
            Debug.LogWarning("[DayEvnet] 판매 가능한 빵이 없습니다.");
            MaxCustomer = ProcessedCustomer;
            CheckDayFinish();
            return;
        }

        //손님 한명 들어옴
        ActualCustomer++;
        isCustomerPresent = true;

        int randomIndex = Random.Range(0, SellableBreads.Count);
        BreadType orderedBread = SellableBreads[randomIndex];

        //주문한 빵 차감
        switch (orderedBread)
        {
            case BreadType.DollCake:
                GameManager.Instance.DollCakeCount--;
                break;
            case BreadType.MushroomMuffin:
                GameManager.Instance.MushroomMuffinCount--;
                break;
            case BreadType.SlimePudding:
                GameManager.Instance.SlimePuddingCount--;
                break;
        }

        bool isPackaging = (Random.Range(0, 2) == 1);

        Debug.Log($"[DayEvent] {ActualCustomer}번째 손님 주문 주문한 빵 : {orderedBread}, 포장 : {isPackaging}");

        //아래에 손님 소환 or 주문UI 코드짜기
        GameObject newCustomer = Instantiate(CustomerPrefab, CustomerSpawnPoint.position, Quaternion.identity);
        Customer customer = newCustomer.GetComponent<Customer>();

        PackagingStation station = FindAnyObjectByType<PackagingStation>();
        DayEventUI dayEvnetUI = FindAnyObjectByType<DayEventUI>();

        if (dayEvnetUI != null && station != null)
        {
            station.FindCustomer(customer);
            //주문UI 출력하기
            dayEvnetUI.OrderedBread(orderedBread, isPackaging);
            customer.SetOrder(orderedBread, isPackaging);
        }
    }

    public void CustomerLeft(int score)
    {
        CustomerScore += score;
        ProcessedCustomer++;
        isCustomerPresent = false;

        Debug.Log($"[DayEvent] 현재까지 돌아간 손님 {ProcessedCustomer} / {MaxCustomer}");

        CheckDayFinish();
    }

    private void CheckDayFinish()
    {
        if (ProcessedCustomer >= MaxCustomer && !DayEventFin)
        {
            DayEventFin = true;
            Debug.Log($"[DayEvnet] 오늘 영업이 끝났습니다.");

            DayEventUI dayEventUI = FindAnyObjectByType<DayEventUI>();
            if (dayEventUI != null)
            {
                dayEventUI.DayFinUI.SetActive(true);
            }
        }
    }

    public void StartNight()
    {
        DayEventFin = false;
    }

    public void StartCleanDayEvent()
    {
        SceneManager.LoadScene("CleanEventScene");
        Debug.Log("[DayEvent] 위생 점검 이벤트로 이동");
    }
}
