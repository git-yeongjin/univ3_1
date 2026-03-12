using TMPro;
using UnityEngine;

public class DayEventUI : MonoBehaviour
{
    private DayEvent DE;
    private GameManager GM;

    public TMP_Text DayCountText;
    public TMP_Text CustomerCountText;

    void Start()
    {
        DE = FindAnyObjectByType<DayEvent>();
        GM = FindAnyObjectByType<GameManager>();
    }


    void Update()
    {
        SettingUI();
    }

    private void SettingUI()
    {
        if (DE != null && GM != null)
        {
            string CurrentTime = "";
            if (GM.Day)
            {
                CurrentTime = "낮";
            }
            else if (GM.Night)
            {
                CurrentTime = "밤";
            }
            if (DayCountText != null)
            {
                DayCountText.text = $"{CurrentTime} / {GM.DayCount}일 차";
            }

            if (CustomerCountText != null)
            {
                CustomerCountText.text = $"남은 손님 : {DE.MaxCustomer}명";
            }
        }
    }
}
