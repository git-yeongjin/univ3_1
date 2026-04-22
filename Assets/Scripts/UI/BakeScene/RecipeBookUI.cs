using UnityEngine;
using UnityEngine.UI;

public class RecipeBookUI : MonoBehaviour
{
    [Header("BakeEventUI 연결")]
    public BakeEventUI bakeEventUI;

    [Header("레시피 북 설정")]
    //레시피 북 상태확인
    public bool isOpenRecipeBook = false;
    //레시피 북 전체UI
    public GameObject RecipeBookPanel;
    //레시피 이미지 모음
    public Sprite[] Sprites;
    //스프라이트 들어갈 이미지
    public Image RecipeImage;
    //튜토리얼 용
    public Image RecipeImage_Tuto;
    //현제 레시피 이미지 인덱스
    public int CurrentRecipeSprite = 0;
    public RecipeDataBook recipeDataBook;

    [Header("레시피 북 사운드 효과음")]
    public AudioClip BookOpenSound;
    public AudioClip BookCloseSound;
    public AudioClip PageTurnSound;

    void Start()
    {
        //레시피 이미지 초기화
        if (RecipeImage != null && Sprites.Length > 0)
        {
            RecipeImage.sprite = Sprites[CurrentRecipeSprite];
        }

        //레시피 북 끄기
        if (RecipeBookPanel != null) RecipeBookPanel.SetActive(false);

        //0일차 여부에 이미지 교체
        if (GameManager.Instance != null && GameManager.Instance.DayCount == 0)
        {
            if (RecipeImage != null) RecipeImage.gameObject.SetActive(false);
            if (RecipeImage_Tuto != null) RecipeImage_Tuto.gameObject.SetActive(true);
        }
        else
        {
            if (RecipeImage != null) RecipeImage.gameObject.SetActive(true);
            if (RecipeImage_Tuto != null) RecipeImage_Tuto.gameObject.SetActive(false);
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isOpenRecipeBook) CloseRecipeBook();
            else OpenRecipeBook();
        }
    }

    public void OpenRecipeBook()
    {
        if (!isOpenRecipeBook)
        {
            if (SoundManager.Instance != null && BookOpenSound != null)
                SoundManager.Instance.PlaySFX(BookOpenSound);

            RecipeBookPanel.SetActive(true);
            isOpenRecipeBook = true;

            if (bakeEventUI != null)
            {
                bakeEventUI.CheckTutorialMission(4);
            }
        }
    }

    public void CloseRecipeBook()
    {
        if (isOpenRecipeBook)
        {
            if (SoundManager.Instance != null && BookCloseSound != null)
                SoundManager.Instance.PlaySFX(BookCloseSound);

            RecipeBookPanel.SetActive(false);
            isOpenRecipeBook = false;
        }
    }

    public void OnClickRight()
    {
        if (GameManager.Instance != null && GameManager.Instance.DayCount == 0) return;

        if (SoundManager.Instance != null && PageTurnSound != null)
            SoundManager.Instance.PlaySFX(PageTurnSound);

        CurrentRecipeSprite++;
        if (CurrentRecipeSprite >= Sprites.Length) CurrentRecipeSprite = 0;

        if (RecipeImage != null) RecipeImage.sprite = Sprites[CurrentRecipeSprite];
    }

    public void OnClickLeft()
    {
        if (GameManager.Instance != null && GameManager.Instance.DayCount == 0) return;

        if (SoundManager.Instance != null && PageTurnSound != null)
            SoundManager.Instance.PlaySFX(PageTurnSound);

        CurrentRecipeSprite--;
        if (CurrentRecipeSprite < 0) CurrentRecipeSprite = Sprites.Length - 1;

        if (RecipeImage != null) RecipeImage.sprite = Sprites[CurrentRecipeSprite];
    }
}
