using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class DayEventUI : MonoBehaviour
{
    private DayEvent DE;
    private GameManager GM;
    private PackagingStation CurrentStation;

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

    private Coroutine HideTextCoroutine;

    void Start()
    {
        DE = FindAnyObjectByType<DayEvent>();
        GM = FindAnyObjectByType<GameManager>();

        PackagingUI.SetActive(false);
        OrderDetailPanel.SetActive(false);
        DayFinUI.SetActive(false);
        OrderedBreadWindow.SetActive(false);
    }

    void Update()
    {
        SettingUI();
    }

    private void SettingUI()
    {
        if (DE != null && GM != null)
        {
            if (CustomerCountText != null)
            {
                CustomerCountText.text = $"남은 손님\n{DE.MaxCustomer - DE.ActualCustomer}명, 카운트 : {DE.CustomerScore}";
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
        OrderedBreadWindow.SetActive(true);
        OrderedBreadText.text = $"주문한 빵 : {order} / 포장 : {isPackaging}";

        if (OrderedBreadText != null) OrderedBreadText.gameObject.SetActive(true);
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

    /*
        public void OnClickPackageYes()
        {
            if (PackagingUI != null) PackagingUI.SetActive(false);

            if (CurrentStation != null)
            {
                CurrentStation.ProceedPackaging();
            }

            Player player = FindAnyObjectByType<Player>();
            if (player != null)
            {
                player.LockCursor(true);
            }
        }

        public void OnClickPackageNo()
        {
            if (PackagingUI != null) PackagingUI.SetActive(false);

            if (CurrentStation != null)
            {
                CurrentStation.CancelPackaging();
            }

            Player player = FindAnyObjectByType<Player>();
            if (player != null)
            {
                player.LockCursor(true);
            }
        }
    */

    public void CloseOrderDetail()
    {
        if (OrderDetailPanel != null) OrderDetailPanel.SetActive(false);
    }

    public void OpenOrderDetail()
    {
        if (OrderDetailPanel != null) OrderDetailPanel.SetActive(true);
    }

    public void OpenNightEventScene()
    {
        if (GM == null) return;

        GM.ChangeDayNight();
        SceneManager.LoadScene("NightEventScene");
    }

    private IEnumerator HideOrderedBreadText()
    {
        yield return new WaitForSeconds(3f);

        if (OrderedBreadWindow != null)
        {
            OrderedBreadWindow.SetActive(false);
        }
    }
}
