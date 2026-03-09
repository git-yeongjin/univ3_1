using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance;

    [Header("Recipe Book폴더 -> Bread RecipeBook")]
    public RecipeDataBook recipeDataBook;

    void Awake()
    {
        Instance = this;
    }

    public RecipeData FindRecipe(List<string> dough)
    {
        Debug.Log($"총 {recipeDataBook.AllRecipes.Count}개의 레시피가 있습니다.");
        foreach (RecipeData recipe in recipeDataBook.AllRecipes)
        {
            //도우에 들어간 재료와 레시피에 있는 재료개수가 같은지 확인
            if (recipe.BreadMaterial.Count != dough.Count) continue;

            bool isMatch = true;
            foreach (string material in recipe.BreadMaterial)
            {
                if (!dough.Contains(material))
                {
                    isMatch = false;
                    break;
                }
            }

            if (isMatch)
            {
                Debug.Log($"[{recipe.BreadName}] 레시피를 찾았습니다.");
                return recipe;
            }
        }

        Debug.Log($"맞는 레시피가 없어서 일반 빵");
        return recipeDataBook.NormalBread;
    }
}
