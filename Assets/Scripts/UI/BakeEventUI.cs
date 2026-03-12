using TMPro;
using UnityEngine;

public class BakeEventUI : MonoBehaviour
{
    [Header("레시피 북 설정")]
    public bool isOpenRecipeBook = false;
    public GameObject RecipeBookUI;
    public RecipeDataBook recipeDataBook;


    public GameObject RecipeContentPrefab;
    public Transform RecipeContentTransform;

    void Start()
    {
        RecipeBookUI.SetActive(false);
    }


    void Update()
    {

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

    public void LoadRecipeContent(MoldCategory category)
    {
        for (int i = RecipeContentTransform.childCount - 1; i >= 0; i--)
        {
            Destroy(RecipeContentTransform.GetChild(i).gameObject);
        }

        foreach (RecipeData recipe in recipeDataBook.AllRecipes)
        {
            if (recipe.moldCategory == category)
            {
                GameObject entry = Instantiate(RecipeContentPrefab, RecipeContentTransform);

                TMP_Text content = entry.GetComponentInChildren<TMP_Text>();
                //string.Join() -> 문자열 이어서 출력 ex ", "면 a, b, c
                content.text = $"빵 이름 : {recipe.BreadName}\n" + $"사용하는 틀 : {recipe.RequiredMold}\n" +
                                $"사용 재료 : {string.Join(", ", recipe.BreadMaterial)}";
            }
        }
    }

    public void OnClickCakeRecipe() { LoadRecipeContent(MoldCategory.Cake); }
    public void OnClickPuddingRecipe() { LoadRecipeContent(MoldCategory.Pudding); }
    public void OnClickMuffinRecipe() { LoadRecipeContent(MoldCategory.Muffin); }
}
