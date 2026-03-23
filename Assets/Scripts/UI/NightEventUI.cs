using UnityEngine;
using UnityEngine.SceneManagement;

public class NightEventUI : MonoBehaviour
{
    private NightEvent NE;
    private GameManager GM;

    [Header("밤 종료 UI")]
    public GameObject NightFinUI;

    void Start()
    {
        NE = FindAnyObjectByType<NightEvent>();
        GM = FindAnyObjectByType<GameManager>();

        NightFinUI.SetActive(false);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            NightFinUI.SetActive(true);
        }
    }

    public void OpenDayEventScene()
    {
        if (GM == null) return;
        GM.ChangeDayNight();

        if (NE != null && NE.isCreatureUnlockedToday && GM.DayCount != 1)
        {
            Debug.Log("새로운 크리쳐 해금으로 빵 제작씬으로 이동");
            SceneManager.LoadScene("BakeEventScene");
        }
        else
        {
            SceneManager.LoadScene("DayEventScene");
        }
    }
}
