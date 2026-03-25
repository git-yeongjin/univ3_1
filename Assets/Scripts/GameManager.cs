using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    private DayEvent DE;
    private NightEvent NE;

    [Header("게임상태")]
    public bool GameStart = false;
    public bool GameFin = false;
    public bool Day = false;
    public bool Night = false;
    //빵 제작 씬 플레이어 조작
    public bool isBakingTime = true;

    [Header("빵 판매가능 여부")]
    public bool DollCake = false;
    public bool MushroomMuffin = false;
    public bool SlimePudding = false;

    [Header("크리쳐 포획 수 만큼 손님 수 증가")]
    public int CustomerToCreature = 1;

    //n일차
    public int DayCount = 0;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        DE = gameObject.GetComponent<DayEvent>();
        NE = gameObject.GetComponent<NightEvent>();
        if (DE == null || NE == null)
        {
            Debug.LogError($"GameManager : DE또는NE가 없습니다.");
        }
        GameStart = true;
        Day = true;
    }

    public void ChangeDayNight()
    {
        if (Day)
        {
            Debug.Log($"낮에서 밤으로 이동합니다.");

            Day = false;
            Night = true;

            //밤으로 넘어가서 초기화
            CustomerToCreature = 0;

            if (NE != null) NE.StartNightEvent();
        }
        else if (Night)
        {
            Debug.Log($"밤에서 낮으로 이동하고 일차가 증가합니다.");

            Day = true;
            Night = false;

            DayCount++;

            if (DE != null)
            {
                DE.ResetDayEvent();
            }
        }
    }

    public void IncreaseCustomer()
    {
        if (CustomerToCreature < 10)
        {
            Debug.Log("크리처를 잡아서 내일 손님이 증가했습니다.");
            CustomerToCreature++;
        }
        else
        {
            Debug.Log("최대 수에 도달했습니다.");
        }
    }
}
