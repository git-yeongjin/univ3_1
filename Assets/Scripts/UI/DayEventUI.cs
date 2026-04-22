using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DayEventUI : MonoBehaviour
{
    private DayEvent DE;
    private GameManager GM;
    private PackagingStation CurrentStation;

    [Header("사운드")]
    public AudioClip UIClickSound;
    public AudioClip BillOpenSound;
    public AudioClip BillCloseSound;

    [Header("일차 이미지")]
    public Sprite[] DayCountSprites;
    public Image DayTensImage;
    public Image DayOnesImage;

    [Header("케이크 주문 이미지")]
    //케이크만 열린 이미지
    public Sprite[] BillCakeSprites_1st;
    //머핀까지 열린 이미지
    public Sprite[] BillCakeSprites_2nd;
    //푸딩까지 열린 이미지
    public Sprite[] BillCakeSprites_3rd;
    [Header("머핀 주문 이미지")]
    //머핀까지 열린 이미지
    public Sprite[] BillMuffinSprites_1st;
    //푸딩까지 열린 이미지
    public Sprite[] BillMuffinSprites_2nd;
    [Header("푸딩 주문 이미지")]
    //푸딩까지 열린 이미지
    public Sprite[] BillPuddingSprites_1st;

    [Header("주문 이미지 적용")]
    public Image BillImage;

    public TMP_Text CustomerCountText;

    public GameObject OrderedBreadWindow;
    //주문 텍스트
    public TMP_Text OrderedBreadText;
    //포장 여부 선택창
    public GameObject PackagingUI;
    [Header("주문 내역 UI")]
    public GameObject OrderDetailPanel;

    [Header("낮 종료 UI")]
    public GameObject DayFinUI;
    public TMP_Text FinCustomerCount;

    [Header("엔딩 연출 설정")]
    public CanvasGroup EndingFadeGroup;     // 화면을 까맣게 만들 페이드 패널
    public GameObject EndingDialogueUI;     // 까만 화면 위에 띄울 대사창 UI
    public TMP_Text EndingDialogueText;     // 대사 텍스트

    [Header("엔딩 이미지")]
    public GameObject GoodEnding;
    public GameObject BadEnding;

    [Header("엔딩 대사 목록")]
    [TextArea]
    public string[] GoodEndingDialogues = {
        "마을 사람들이 모두 사랑둥이가 되었어!",
        "가게를 차려서 모두를 사랑둥이로 만들겠다는 꿈에 한 발짝 다가간거야!",
        "너무 행복해",
        "앞으로도 더 많은 사람들을 사랑둥이로 만들자",
        "세상 사람들 모두를 사랑둥이로 만드는 거야!"
    };

    [TextArea]
    public string[] BadEndingDialogues = {
        "가게를 차려서 세상 사람들 모두를 사랑둥이로 만든다!",
        "...라는 원대한 꿈을 결국 이루지 못했다",
        "하지만 괜찮아!",
        "내 옆엔 사랑둥이로 변해버린 자기도 있고",
        "모두가 사랑둥이인 세상에 가면 된다",
        "세상 사람들을 사랑둥이로 만드는 꿈이 지금은 어려웠지만",
        "괜찮다",
        "기회는 언젠가 다시 오기 마련이니까"
    };

    [Header("손님 대사 데이터")]
    public string[] HallDialogues = {
        "매장이 너무 예뻐요!",
        "먹고 가려고요",
        "여기서 먹고 가도 되죠?"
    };

    public string[] PackagingDialogues = {
        "포장도 가능한가요?",
        "집에 가져가서 먹으려고요!",
        "ㅁㅁ 포장해주세요"
    };

    public string[] CommonDialogues = {
        "지금 영업 하나요? ㅁㅁ 하나 주세요",
        "좋은 냄새 난다! ㅁㅁ하나 주시겠어요?",
        "ㅁㅁ 주세요",
        "**...아니 ㅁㅁ이요. 아 아닌가? **? 그냥 ㅁㅁ으로 주세요",
        "ㅁㅁ 주세요. 어 자기야 주문하느라 응"
    };

    public string[] LateCommonDialogues = {
        "요즘 길에 사람이 적어진 것 같아요. ㅁㅁ 주세요",
        "동네가 좀 한산해진 느낌이에요. 장사는 잘 되나요?"
    };

    private Coroutine HideTextCoroutine;

    void Start()
    {
        DE = FindAnyObjectByType<DayEvent>();
        GM = FindAnyObjectByType<GameManager>();

        PackagingUI.SetActive(false);
        OrderDetailPanel.SetActive(false);
        DayFinUI.SetActive(false);
        OrderedBreadWindow.SetActive(false);
        if (GoodEnding != null) GoodEnding.SetActive(false);
        if (BadEnding != null) BadEnding.SetActive(false);
    }

    void Update()
    {
        SettingUI();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (OrderDetailPanel != null && OrderDetailPanel.activeSelf)
            {
                if (SoundManager.Instance != null && BillOpenSound != null)
                {
                    SoundManager.Instance.PlaySFX(BillOpenSound);
                }
            }
            else
            {
                if (SoundManager.Instance != null && BillCloseSound != null)
                {
                    SoundManager.Instance.PlaySFX(BillCloseSound);
                }
            }
            if (OrderDetailPanel != null) OrderDetailPanel.SetActive(!OrderDetailPanel.activeSelf);
        }
    }

    private void SettingUI()
    {
        if (DE != null && GM != null)
        {
            if (CustomerCountText != null)
            {
                //CustomerCountText.text = $"남은 손님\n{DE.MaxCustomer - DE.ActualCustomer}명, 카운트 : {DE.CustomerScore}";
                CustomerCountText.text = $"{DE.CurrentCustomerScore}";
            }
        }

        if (DayTensImage.sprite != null && DayOnesImage != null && DayCountSprites.Length >= 10)
        {
            int tens = (GM.DayCount / 10) % 10;
            int ones = GM.DayCount % 10;

            DayTensImage.sprite = DayCountSprites[tens];
            DayOnesImage.sprite = DayCountSprites[ones];
        }
    }

    public void OrderedBread(BreadType order, bool isPackaging)
    {
        OrderDetailPanel.SetActive(true);
        if (HideTextCoroutine != null) StopCoroutine(HideTextCoroutine);

        HideTextCoroutine = StartCoroutine(HideOrderedBreadText());

        switch (order)
        {
            //케이크 주문 시
            case BreadType.DollCake:
                //주문 전부 해금
                if (GM.DollCake && GM.MushroomMuffin && GM.SlimePudding)
                {
                    BillImage.sprite = isPackaging ? BillCakeSprites_3rd[1] : BillCakeSprites_3rd[0];
                }
                //머핀 까지 해금
                else if (GM.DollCake && GM.MushroomMuffin)
                {
                    BillImage.sprite = isPackaging ? BillCakeSprites_2nd[1] : BillCakeSprites_2nd[0];
                }
                //케이크만
                else if (GM.DollCake)
                {
                    BillImage.sprite = isPackaging ? BillCakeSprites_1st[1] : BillCakeSprites_1st[0];
                }
                break;
            //머핀 주문 시
            case BreadType.MushroomMuffin:
                if (GM.DollCake && GM.MushroomMuffin && GM.SlimePudding)
                {
                    BillImage.sprite = isPackaging ? BillMuffinSprites_2nd[1] : BillMuffinSprites_2nd[0];
                }
                else if (GM.DollCake && GM.MushroomMuffin)
                {
                    BillImage.sprite = isPackaging ? BillMuffinSprites_1st[1] : BillMuffinSprites_1st[0];
                }
                break;
            //푸딩 주문 시
            case BreadType.SlimePudding:
                if (GM.DollCake && GM.MushroomMuffin && GM.SlimePudding)
                {
                    BillImage.sprite = isPackaging ? BillPuddingSprites_1st[1] : BillPuddingSprites_1st[0];
                }
                break;
        }
    }

    public void ShowPackagingUI(PackagingStation station)
    {
        CurrentStation = station;
        if (PackagingUI != null) PackagingUI.SetActive(true);

        Player player = FindAnyObjectByType<Player>();
        if (player != null)
        {
            player.LockCursor(false);
        }
    }

    public void CloseOrderDetail()
    {
        if (SoundManager.Instance != null && BillCloseSound != null)
        {
            SoundManager.Instance.PlaySFX(BillCloseSound);
        }

        if (OrderDetailPanel != null) OrderDetailPanel.SetActive(false);
    }

    public void OpenOrderDetail()
    {
        if (SoundManager.Instance != null && BillOpenSound != null)
        {
            SoundManager.Instance.PlaySFX(BillOpenSound);
        }

        if (OrderDetailPanel != null) OrderDetailPanel.SetActive(true);
    }

    public void OpenNightEventScene()
    {
        if (GM == null) return;

        GM.ChangeDayNight();

        if (SoundManager.Instance != null && UIClickSound != null)
        {
            SoundManager.Instance.PlaySFX(UIClickSound);
        }

        if (GameManager.Instance.DayCount != 15)
            LoadingUIManager.Instance.LoadScene("NightEventScene");
    }

    private IEnumerator HideOrderedBreadText()
    {
        yield return new WaitForSeconds(3f);

        if (OrderedBreadWindow != null)
        {
            OrderedBreadWindow.SetActive(false);
        }
    }

    public void CheckGameEnding()
    {
        int totalScore = GameManager.Instance.CustomerScore;

        Debug.Log($"[엔딩 정산] 최종 누적 점수 : {totalScore}");

        StartCoroutine(EndingSequenceRoutine(totalScore));
    }

    private IEnumerator EndingSequenceRoutine(int totalScore)
    {
        bool isGoodEnding = totalScore >= 150;
        string[] selectedDialogues = isGoodEnding ? GoodEndingDialogues : BadEndingDialogues;
        GameObject selectedEndingImage = isGoodEnding ? GoodEnding : BadEnding;

        if (EndingFadeGroup != null)
        {
            EndingFadeGroup.gameObject.SetActive(true);
            float timer = 0f;
            while (timer < 1.5f)
            {
                timer += Time.deltaTime;
                EndingFadeGroup.alpha = Mathf.Lerp(0f, 1f, timer / 1.5f);
                yield return null;
            }
            EndingFadeGroup.alpha = 1f;
        }

        if (EndingDialogueUI != null)
        {
            EndingDialogueUI.SetActive(true);

            foreach (string dialogue in selectedDialogues)
            {
                if (EndingDialogueText != null) EndingDialogueText.text = dialogue;

                // 한 프레임 대기
                yield return null;

                // 마우스 좌클릭 또는 스페이스바를 누를 때까지 코루틴 일시정지
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
            }

            //대사가 모두 끝났으니 대사창 끄기
            EndingDialogueUI.SetActive(false);

            //까만 화면 뒤에서 엔딩 이미지 켜두기
            if (selectedEndingImage != null) selectedEndingImage.SetActive(true);
        }

        //화면 다시 밝아지기 (페이드 아웃) -> 엔딩 일러스트가 드러남
        if (EndingFadeGroup != null)
        {
            float timer = 0f;
            while (timer < 1.5f)
            {
                timer += Time.deltaTime;
                EndingFadeGroup.alpha = Mathf.Lerp(1f, 0f, timer / 1.5f);
                yield return null;
            }
            EndingFadeGroup.alpha = 0f;
            EndingFadeGroup.gameObject.SetActive(false);
        }
    }


    public string GetCustomerDialogue(BreadType orderedBread, bool isPackaging)
    {
        List<string> dialoguePool = new List<string>();

        if (isPackaging)
        {
            dialoguePool.AddRange(PackagingDialogues);
        }
        else
        {
            dialoguePool.AddRange(HallDialogues);
        }

        if (GameManager.Instance.DayCount >= 7)
        {
            // 7일차 이후면 후반용 대사 사용 (기존 대사 미사용)
            dialoguePool.AddRange(LateCommonDialogues);
        }
        else
        {
            // 7일차 이전이면 기존 공통 대사 사용
            dialoguePool.AddRange(CommonDialogues);
        }

        //풀 안의 대사들(상황별 + 공통) 중 딱 하나만 랜덤으로 뽑기
        string selectedDialogue = dialoguePool[Random.Range(0, dialoguePool.Count)];

        string breadName = GetBreadNameKorean(orderedBread);
        selectedDialogue = selectedDialogue.Replace("ㅁㅁ", breadName);

        if (selectedDialogue.Contains("**"))
        {
            string otherBread = GetRandomOtherBreadName(orderedBread);
            selectedDialogue = selectedDialogue.Replace("**", otherBread);
        }

        return selectedDialogue;
    }

    private string GetBreadNameKorean(BreadType type)
    {
        switch (type)
        {
            case BreadType.DollCake: return "인형 케이크";
            case BreadType.MushroomMuffin: return "버섯 머핀";
            case BreadType.SlimePudding: return "슬라임 푸딩";
            case BreadType.Rollcake: return "롤케이크";
            default: return "빵";
        }
    }

    private string GetRandomOtherBreadName(BreadType excludeType)
    {
        List<string> others = new List<string>();
        if (excludeType != BreadType.DollCake) others.Add("인형 케이크");
        if (excludeType != BreadType.MushroomMuffin) others.Add("버섯 머핀");
        if (excludeType != BreadType.SlimePudding) others.Add("슬라임 푸딩");

        if (others.Count == 0) return "아무 빵";
        return others[Random.Range(0, others.Count)];
    }
}
