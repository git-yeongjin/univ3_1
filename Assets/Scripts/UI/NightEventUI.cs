using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NightEventUI : MonoBehaviour
{
    private NightEvent NE;
    private GameManager GM;

    [Header("밤 종료 UI")]
    public GameObject NightFinUI;

    [Header("0일차 튜토리얼")]
    public GameObject NightTutorialUI;
    public TMP_Text NightTutorialText;

    private int CurrentDialogueIndex = 0;
    public string[] TutorialDialogues =
    {
        "임시 텍스트"
    };

    void Start()
    {
        NE = FindAnyObjectByType<NightEvent>();
        GM = FindAnyObjectByType<GameManager>();

        if (GameManager.Instance.DayCount == 0)
        {
            NightTutorialUI.SetActive(true);

            if (NightTutorialText != null && TutorialDialogues.Length > 0)
            {
                NightTutorialText.text = TutorialDialogues[0];
                CurrentDialogueIndex = 1;
            }
            else
            {
                NightTutorialUI.SetActive(false);
            }
        }

        NightFinUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            NightFinUI.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (NightTutorialUI != null && NightTutorialUI.activeSelf)
            {
                if (TutorialDialogues == null) return;

                if (CurrentDialogueIndex < TutorialDialogues.Length)
                {
                    Debug.Log($"[BakeEventUI] 대사 출력중 {CurrentDialogueIndex} / {TutorialDialogues.Length}");

                    NightTutorialText.text = TutorialDialogues[CurrentDialogueIndex];
                    CurrentDialogueIndex++;
                }
                else
                {
                    Debug.Log($"[BakeEventUI] 대사 종료");
                    NightTutorialUI.SetActive(false);
                    CurrentDialogueIndex = 0;
                }
            }
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
