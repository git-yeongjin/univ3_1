using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum BreadType
{
    None,
    DollCake,
    MushroomMuffin,
    SlimePudding,
    Rollcake
}

public class DayEvent : MonoBehaviour
{
    [Header("낮 시간 및 스폰 설정")]
    //낮 시간
    public int DayTime = 0;
    //손님 프리팹
    public GameObject CustomerPrefab;
    public GameObject SpecialCustomerPrefab;
    //손님 스폰 위치
    public Transform CustomerSpawnPoint;
    public Transform PickupPoint;

    [Header("손님 등장 타이머")]
    public float MinSpawnDelay = 3.0f;
    public float MaxSpawnDelay = 5.0f;
    private float currentSpawnTimer = 0f;

    [Header("사운드")]
    public AudioClip CustomerSpawnSound;
    public AudioClip ResultPopupSound;

    [Header("손님 및 영업 정보")]
    //현재까지 스폰 된 손님
    public int ActualCustomer = 0;
    //판매 완료한 손님
    public int ProcessedCustomer = 0;
    //총 손님 수
    public int MaxCustomer = 3;
    //손님 카운트
    public int CurrentCustomerScore = 0;
    //영업종료 -> 밤으로 전환
    public bool DayEventFin = false;
    public bool isCustomerPresent = false;

    private bool isDayEventScene = false;

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

        if (isDayEventScene && !DayEventFin && !isCustomerPresent && ActualCustomer < MaxCustomer)
        {
            currentSpawnTimer -= Time.deltaTime;

            if (currentSpawnTimer <= 0f)
            {
                CustomerRandomOrder();

                currentSpawnTimer = Random.Range(MinSpawnDelay, MaxSpawnDelay);
            }
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
            isDayEventScene = true;
            Debug.Log($"[DayEvent] {scene.name} 씬,  낮 영업 세팅을 초기화합니다.");

            // 씬에 만든 스폰 오브젝트 이름
            GameObject spawnObj = GameObject.Find("CustomerSpawnPoint");
            if (spawnObj != null) CustomerSpawnPoint = spawnObj.transform;
            else Debug.LogError("[DayEvent] 씬에서 'CustomerSpawnPoint' 오브젝트를 찾을 수 없습니다.");

            // 씬에 만든 픽업 오브젝트 이름
            GameObject pickupObj = GameObject.Find("PickupPoint");
            if (pickupObj != null) PickupPoint = pickupObj.transform;
            else Debug.LogError("[DayEvent] 씬에서 'PickupPoint' 오브젝트를 찾을 수 없습니다.");

            ResetDayEvent();
        }
        else
        {
            isDayEventScene = false;
        }
    }

    public void ResetDayEvent()
    {
        ActualCustomer = 0;
        ProcessedCustomer = 0;
        CurrentCustomerScore = 0;
        DayEventFin = false;
        hasTriggeredCleanEvent = false;
        isCustomerPresent = false;
        currentSpawnTimer = Random.Range(MinSpawnDelay, MaxSpawnDelay);

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

        int currentDay = GameManager.Instance.DayCount;
        bool isSpecialDay = (currentDay == 7 || currentDay == 9 || currentDay == 11);
        bool isFirstCustomer = (ActualCustomer == 0); // 오늘 온 손님이 0명이면 첫 번째 손님

        BreadType orderedBread = BreadType.None;
        bool isPackaging = false;
        string finalDialogue = "";

        GameObject prefabToSpawn = CustomerPrefab;

        // 조건: 7,9,11일차 이면서 && 오늘의 첫 번째 손님일 때
        if (isSpecialDay && isFirstCustomer)
        {
            ActualCustomer++;
            isCustomerPresent = true;
            isPackaging = false; // 연구원은 매장 식사로 처리

            if (SpecialCustomerPrefab != null)
            {
                prefabToSpawn = SpecialCustomerPrefab;
            }
            else
            {
                Debug.LogWarning("[DayEvent] 특수 손님 프리팹이 등록되지 않아 일반 손님으로 대체합니다.");
            }

            if (currentDay == 7)
            {
                orderedBread = BreadType.DollCake; // 푸딩 주문
                // \n 으로 기획서의 A/B 말풍선을 한 번에 띄어쓰기해서 보여줍니다.
                finalDialogue = "<color=#fbe9ff>안녕하신지요~ 빵집이 생겼다길래 호다닥 달려 왔어요~\n오! 케이크가 맛있어보이네요!</color>";
            }
            else if (currentDay == 9)
            {
                orderedBread = BreadType.MushroomMuffin; // 케이크 주문
                finalDialogue = "<color=#fbe9ff>안녕하세요~ 저번에 케이크 너무 너무 맛있길래 또 왔어요~ 와하하! 엄청 크던데요~\n오늘은 머핀 먹을래요</color>";
            }
            else if (currentDay == 11)
            {
                orderedBread = BreadType.SlimePudding; // 머핀 주문
                finalDialogue = "<color=#fbe9ff>안녕하세요오오 푸딩. 주세요</color>";
            }

            Debug.Log($"[DayEvent] 특수 손님(연구원)이 등장했습니다! 주문: {orderedBread}");
        }
        else
        {
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

            if (SoundManager.Instance != null && CustomerSpawnSound != null)
            {
                SoundManager.Instance.PlaySFX(CustomerSpawnSound);
            }

            int randomIndex = Random.Range(0, SellableBreads.Count);
            orderedBread = SellableBreads[randomIndex];
            isPackaging = (Random.Range(0, 2) == 1);

            DayEventUI dayEventUI = FindAnyObjectByType<DayEventUI>();
            if (dayEventUI != null)
            {
                finalDialogue = dayEventUI.GetCustomerDialogue(orderedBread, isPackaging);
            }
        }

        if (CustomerSpawnPoint == null)
        {
            Debug.LogError("[DayEvent] 스폰 포인트가 설정되지 않아 손님을 소환할 수 없습니다.");
            return;
        }

        Debug.Log($"[DayEvent] {ActualCustomer}번째 손님 주문 주문한 빵 : {orderedBread}, 포장 : {isPackaging}");

        //아래에 손님 소환 or 주문UI 코드짜기
        GameObject newCustomer = Instantiate(prefabToSpawn, CustomerSpawnPoint.position, Quaternion.identity);
        Customer customer = newCustomer.GetComponent<Customer>();
        PackagingStation station = FindAnyObjectByType<PackagingStation>();

        Vector3 targetPosition = PickupPoint != null ? PickupPoint.position : CustomerSpawnPoint.position;

        if (station != null && customer != null)
        {
            station.FindCustomer(customer);
            customer.SetOrder(orderedBread, isPackaging, finalDialogue, targetPosition);
        }
    }

    public void SellBread(BreadType soldBread)
    {
        //주문한 빵 차감
        switch (soldBread)
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

        UpdateAllShowcases();
    }

    public void CustomerLeft(int score)
    {
        CurrentCustomerScore += score;
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
                if (SoundManager.Instance != null && ResultPopupSound != null)
                {
                    SoundManager.Instance.PlaySFX(ResultPopupSound);
                }
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
        //SceneManager.LoadScene("CleanEventScene");
        LoadingUIManager.Instance.LoadScene("CleanEventScene");
        Debug.Log("[DayEvent] 위생 점검 이벤트로 이동");
    }

    private void UpdateAllShowcases()
    {
        ShowCase[] allShowcases = FindObjectsByType<ShowCase>(FindObjectsSortMode.None);

        foreach (ShowCase showcase in allShowcases)
        {
            showcase.DisplayBread();
        }
    }
}
