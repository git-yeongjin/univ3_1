using UnityEngine;
using TMPro;

public class CleanDayUI : MonoBehaviour
{
    //위생 점검 제한시간 가져오기
    private DayEvent DE;

    [Header("위생 관리 타이머")]
    public TMP_Text CleanDayEvent_TimeLimitText;

    void Start()
    {
        DE = FindAnyObjectByType<DayEvent>();
        if (DE == null)
        {
            Debug.LogError("DayEvent스크립트를 찾을 수 없음");
        }
    }


    void Update()
    {
        if (CleanDayEvent_TimeLimitText != null)
        {
            CleanDayEvent_TimeLimitText.text = DE.CleanDayEvent_TimeLimit.ToString("F0");
        }
    }
}
