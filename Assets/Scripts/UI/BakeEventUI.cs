using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BakeEventUI : MonoBehaviour
{
    private GameManager GM;

    [Header("레시피 북 설정")]
    public bool isOpenRecipeBook = false;
    public GameObject RecipeBookUI;
    public Sprite[] Sprites;
    public Image RecipeImage;
    public int CurrentRecipeSprite = 0;
    public RecipeDataBook recipeDataBook;

    public GameObject BakeFailUI;

    void Start()
    {
        GM = FindAnyObjectByType<GameManager>();
        if (GM == null) Debug.LogError($"GameManager를 찾을 수 없습니다.");

        if (RecipeImage != null && Sprites.Length > 0)
        {
            RecipeImage.sprite = Sprites[CurrentRecipeSprite];
        }

        RecipeBookUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClickBakeFinish();
        }
    }

    public void OpenRecipeBook()
    {
        if (!isOpenRecipeBook)
        {
            RecipeBookUI.SetActive(true);
            isOpenRecipeBook = true;
        }
    }

    public void CloseRecipeBook()
    {
        if (isOpenRecipeBook)
        {
            RecipeBookUI.SetActive(false);
            isOpenRecipeBook = false;
        }
    }

    public void OnClickRight()
    {
        if (CurrentRecipeSprite < Sprites.Length - 1)
        {
            CurrentRecipeSprite++;
            RecipeImage.sprite = Sprites[CurrentRecipeSprite];
        }
        else
        {
            CurrentRecipeSprite = 0;
            RecipeImage.sprite = Sprites[CurrentRecipeSprite];
        }
    }

    public void OnClickLeft()
    {
        if (CurrentRecipeSprite > 0)
        {
            CurrentRecipeSprite--;
            RecipeImage.sprite = Sprites[CurrentRecipeSprite];
        }
        else
        {
            CurrentRecipeSprite = Sprites.Length - 1;
            RecipeImage.sprite = Sprites[CurrentRecipeSprite];
        }
    }

    public void OnClickBakeFinish()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit HitInfo;

        if (Physics.Raycast(ray, out HitInfo))
        {
            Debug.Log("hit info : " + HitInfo.collider.gameObject.name);

            if (HitInfo.collider.gameObject.name == "반죽완료" && GM.isBakingTime)
            {
                Dough currentDought = FindAnyObjectByType<Dough>();
                if (currentDought != null)
                {
                    currentDought.FindRecipe();
                    if (currentDought.recipe == null)
                    {
                        Debug.Log("[BakeEventUI] 재료가 맞지 않아 반죽을 섞지 못했습니다.");
                    }
                    else
                    {
                        Debug.Log("반죽을 섞었습니다.");
                        GM.isBakingTime = false;
                    }
                }
            }
        }
    }

    public void OpenDayEventScene()
    {
        SceneManager.LoadScene("DayEventScene");
    }
}
