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

    [Header("텍스트")]
    //낮인지 밤인지 적는 텍스트
    public TMP_Text DayText;
    public TMP_Text CleanDayEvent_TimeLimitText;

    //n일차
    public int DayCount = 0;

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
        SettingUI();

        if (Input.GetKeyDown(KeyCode.N))
        {
            ChangeDayNight();
        }
    }

    private void SettingUI()
    {
        string CurrentTime = "";
        if (Day)
        {
            CurrentTime = "낮";
        }
        else if (Night)
        {
            CurrentTime = "밤";
        }

        DayText.text = CurrentTime + DayCount.ToString() + "일 차";
        CleanDayEvent_TimeLimitText.text = DE.CleanDayEvent_TimeLimit.ToString("F0");
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
