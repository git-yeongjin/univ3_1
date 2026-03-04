using UnityEngine;

public class DayEvent : MonoBehaviour
{
    [Header("낮 이벤트")]
    //n일차
    public int DayCount = 1;
    //손님 수
    public int Sonnim = 0;
    //수익
    public int Coin = 0;
    //영업종료 -> 밤으로 전환
    public bool DayEventFin = false;

    [Header("빵 이벤트")]
    public bool BbangEvent = false;

    //빵 완성도
    public int BbangScore = 0;
    //빵 판매갯수
    public int BbangSellCount = 0;

    [Header("위생점검 이벤트")]
    //8일차에 활성화
    public bool CleanDayEvent = false;
    //이벤트 통과 확인
    public bool CleanDayEvent_Clear = false;

    void Start()
    {
        BbangSetting();
    }

    void Update()
    {

    }

    //빵 레시피 적는 곳
    private void BbangSetting()
    {

    }
}
