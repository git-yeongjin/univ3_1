using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayEventUI : MonoBehaviour
{
    private DayEvent DE;
    private GameManager GM;

    public Sprite[] DayCountSprites;
    public Image DayCountImage;

    public TMP_Text CustomerCountText;

    void Start()
    {
        DE = FindAnyObjectByType<DayEvent>();
        GM = FindAnyObjectByType<GameManager>();

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
                CustomerCountText.text = $"남은 손님 : {DE.MaxCustomer}명";
            }
        }

        if (DayCountImage != null && DayCountSprites.Length > 0)
        {
            int spriteIndex = GM.DayCount;

            spriteIndex = Mathf.Clamp(spriteIndex, 0, DayCountSprites.Length - 1);

            DayCountImage.sprite = DayCountSprites[spriteIndex];
        }
    }
}
