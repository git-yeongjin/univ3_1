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

    [Header("빵 판매가능 여부")]
    public bool DollCake = false;
    public bool MushroomMuffin = false;
    public bool SlimePudding = false;

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            ChangeDayNight();
        }
    }

    private void ChangeDayNight()
    {
        if (Day)
        {
            Day = false;
            Night = true;
        }
        else if (Night)
        {
            Day = true;
            Night = false;

            DayCount++;
        }
    }
}
