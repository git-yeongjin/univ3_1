using UnityEngine;

public class DayEvent : MonoBehaviour
{
    private GameManager GM;

    [Header("낮 이벤트")]
    //빵 레시피는 GameManager에서 BbangSetting에서 설정
    //낮 시간
    public int DayTime = 0;
    //손님 수
    public int Sonnim = 0;
    //수익
    public int Money = 0;
    //영업종료 -> 밤으로 전환
    public bool DayEventFin = false;

    [Header("빵 이벤트")]
    public bool BbangEvent = false;

    //빵 완성도
    //public int BbangScore = 0;
    //빵 판매갯수
    public int BbangSellCount = 0;

    [Header("위생점검 이벤트")]
    //8일차에 활성화
    public bool CleanDayEvent = false;
    //이벤트 통과 확인
    public bool CleanDayEvent_Clear = false;

    void Start()
    {
        GM = gameObject.GetComponent<GameManager>();
        if (GM == null)
        {
            Debug.LogError($"GM을 못찾음");
        }
    }

    void Update()
    {
        if (DayEventFin)
        {
            //밤으로 넘어가는 UI가 뜨고 버튼 누르면 StartNight실행
        }
    }

    public void BbangBake()
    {
        //
    }

    public void StartNight()
    {
        //밤 씬으로 이동하기
        //이동했으니 다시 false
        DayEventFin = false;
    }
}
