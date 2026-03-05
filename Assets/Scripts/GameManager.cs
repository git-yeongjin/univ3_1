using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("게임상태")]
    public bool GameStart = false;
    public bool GameFin = false;
    public bool Day = false;
    public bool Night = false;

    [Header("텍스트")]
    //낮인지 밤인지 적는 텍스트
    public TMP_Text DayorNight;

    //n일차
    public int DayCount = 0;

    void Start()
    {
        BbangSetting();
        Day = true;
    }

    void Update()
    {

    }

    private void BbangSetting()
    {

    }
}
