using UnityEngine;
using TMPro;

public class CleanDayUI : MonoBehaviour
{
    //위생 점검 제한시간 가져오기
    private CleanEvent CE;

    [Header("위생 관리 타이머")]
    public TMP_Text CleanDayEvent_TimeLimitText;

    [Header("크리쳐 흔적 발견 시 엔딩")]
    public GameObject CleanEnding;

    void Start()
    {
        CE = FindAnyObjectByType<CleanEvent>();
        if (CE == null)
        {
            Debug.LogError("DayEvent스크립트를 찾을 수 없음");
        }
        if (CleanEnding != null) CleanEnding.SetActive(false);
    }


    void Update()
    {
        if (CleanDayEvent_TimeLimitText != null)
        {
            CleanDayEvent_TimeLimitText.text = CE.CleanDayEvent_TimeLimit.ToString("F0");
        }
    }
}
