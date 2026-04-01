using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private DayEvent DE;
    private NightEvent NE;

    [Header("게임상태")]
    public bool GameStart = false;
    public bool GameFin = false;
    public bool Day = false;
    public bool Night = false;
    //빵 제작 씬 플레이어 조작
    public bool isBakingTime = false;

    [Header("빵 판매가능 여부")]
    public bool DollCake = false;
    public bool MushroomMuffin = false;
    public bool SlimePudding = false;

    [Header("최대 판매 갯수")]
    public int DollCakeCount = 0;
    public int MushroomMuffinCount = 0;
    public int SlimePuddingCount = 0;

    [Header("위생 관리 이벤트 보상&패널티")]
    public bool CustomerCountDoubleEvent = false;
    public bool CustomerCountPenaltyEvent = false;

    [Header("일차 및 손님 설정")]
    public int DayCount = 0;
    public int CustomerToCreature = 1;
    private readonly int MaxCustomerLimit = 10;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BakeEventScene")
        {
            isBakingTime = true;
            Debug.Log($"[GameManager] {scene.name} 씬, isBakingTime = {isBakingTime}");
        }
        else
        {
            isBakingTime = false;
            Debug.Log($"[GameManager] {scene.name} 씬, isBakingTime = {isBakingTime}");
        }
    }

    void Start()
    {
        DE = GetComponent<DayEvent>();
        NE = GetComponent<NightEvent>();

        if (DE == null || NE == null)
        {
            Debug.LogError($"[GameManager] DayEvent 또는 NightEvent를 찾을 수 없습니다.");
        }

        GameStart = true;
        Day = true;
        Night = false;
    }

    public void ChangeDayNight()
    {
        if (Day)
        {
            Debug.Log($"[GameManager] 낮에서 밤으로 이동합니다.");
            Day = false;
            Night = true;

            //위생 이벤트 버프&디버프, 손님 카운트 초기화
            CustomerCountDoubleEvent = false;
            CustomerCountPenaltyEvent = false;
            CustomerToCreature = 0;

            if (NE != null) NE.StartNightEvent();
        }
        else if (Night)
        {
            Debug.Log($"[GameManager] 밤에서 낮으로 이동하고 일차가 증가합니다.");
            Day = true;
            Night = false;

            DayCount++;

            if (DE != null) DE.ResetDayEvent();
        }
    }

    public void IncreaseCustomer()
    {
        if (CustomerToCreature < MaxCustomerLimit)
        {
            Debug.Log("[GameManager] 크리처를 잡아서 내일 손님이 증가했습니다.");
            CustomerToCreature++;
        }
        else
        {
            Debug.Log("[GameManager] 최대 수에 도달했습니다.");
        }
    }
}
