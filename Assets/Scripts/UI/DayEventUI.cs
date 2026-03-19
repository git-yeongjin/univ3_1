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

    public Sprite[] DayCountSprites;
    public Image DayCountImage;

    public TMP_Text CustomerCountText;

    //주문 텍스트
    public TMP_Text OrderedBreadText;
    //포장 여부 선택창
    public GameObject PackagingUI;
    [Header("주문 내역 UI")]
    public GameObject OrderDetailPanel;
    public TMP_Text OrderDetilText;
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

        if (DayCountSprites != null && DayCountImage != null)
        {
            DayCountImage.sprite = DayCountSprites[0];
        }
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
                CustomerCountText.text = $"온 손님\n{DE.Customer}명";
            }
        }

        if (DayCountImage != null && DayCountSprites.Length > 0)
        {
            int spriteIndex = GM.DayCount;

            spriteIndex = Mathf.Clamp(spriteIndex, 0, DayCountSprites.Length - 1);

            DayCountImage.sprite = DayCountSprites[spriteIndex];
        }
    }

    public void OrderedBread(BreadType order, bool isPackaging)
    {
        OrderedBreadText.text = $"주문한 빵 : {order} / 포장 : {isPackaging}";
        OrderDetilText.text = $"주문한 빵 : {order} / 포장 : {isPackaging}";

        if (OrderedBreadText != null) OrderedBreadText.gameObject.SetActive(true);
        if (HideTextCoroutine != null) StopCoroutine(HideTextCoroutine);

        HideTextCoroutine = StartCoroutine(HideOrderedBreadText());
    }

    public void ShowPackagingUI(PackagingStation station)
    {
        CurrentStation = station;
        if (PackagingUI != null) PackagingUI.SetActive(true);
    }

    public void OnClickPackageYes()
    {
        if (PackagingUI != null) PackagingUI.SetActive(false);

        if (CurrentStation != null)
        {
            CurrentStation.ProceedPackaging();
        }
    }

    public void OnClickPackageNo()
    {
        if (PackagingUI != null) PackagingUI.SetActive(false);

        if (CurrentStation != null)
        {
            CurrentStation.CancelPackaging();
        }
    }

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
        DE.ResetCustomer();
        SceneManager.LoadScene("NightEventScene");
    }

    private IEnumerator HideOrderedBreadText()
    {
        yield return new WaitForSeconds(3f);

        if (OrderedBreadText != null)
        {
            OrderedBreadText.gameObject.SetActive(false);
        }
    }
}
