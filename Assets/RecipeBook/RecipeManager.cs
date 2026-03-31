using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance;

    [Header("Recipe Book폴더 -> Bread RecipeBook")]
    public RecipeDataBook recipeDataBook;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public RecipeData FindRecipe(List<string> doughMaterials)
    {
        if (recipeDataBook == null || recipeDataBook.AllRecipes == null)
        {
            Debug.LogError($"[RecipeManager] 레시피 북 테이터가 비어있습니다.");
            return null;
        }

        Debug.Log($"총 {recipeDataBook.AllRecipes.Count}개의 레시피가 있습니다.");

        foreach (RecipeData recipe in recipeDataBook.AllRecipes)
        {
            //도우에 들어간 재료와 레시피에 있는 재료개수가 같은지 확인
            if (recipe.BreadMaterial.Count != doughMaterials.Count) continue;

            List<string> tempDough = new List<string>(doughMaterials);
            bool isMatch = true;

            foreach (string requiredMaterial in recipe.BreadMaterial)
            {
                if (tempDough.Contains(requiredMaterial))
                {
                    tempDough.Remove(requiredMaterial);
                }
                else
                {
                    isMatch = false;
                    break;
                }
            }

            if (isMatch)
            {
                Debug.Log($"[RecipeManager] [{recipe.BreadName}] 레시피를 찾았습니다.");
                return recipe;
            }
        }

        Debug.Log($"[RecipeManager] 맞는 레시피가 없어서 다시 제작 합니다.");
        return null;
    }
}
