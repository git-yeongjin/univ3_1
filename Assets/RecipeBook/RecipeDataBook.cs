using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Bread RecipeBook", menuName = "Bakery/RecipeBook")]
public class RecipeDataBook : ScriptableObject
{
    [Header("RecipeBook폴더에서 가져오기")]
    //레시피에 없으면 나오는 빵
    public RecipeData NormalBread;
    //빵 레시피 리스트
    public List<RecipeData> AllRecipes;
}
