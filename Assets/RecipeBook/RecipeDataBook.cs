using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[CreateAssetMenu(fileName = "Bread RecipeBook", menuName = "Bakery/RecipeBook")]
public class RecipeDataBook : ScriptableObject
{
    [Header("RecipeBook폴더에서 가져오기")]
    //레시피에 없으면 나오는 빵
    public RecipeData NormalBread;
    //빵 레시피 리스트
    public List<RecipeData> AllRecipes;

#if UNITY_EDITOR
    [ContextMenu("모든 레시피 불러오기")]
    public void LoadAllRecipes()
    {
        AllRecipes.Clear();

        string[] Guids = AssetDatabase.FindAssets("t:RecipeData");

        foreach (string Guid in Guids)
        {
            string AssetPath = AssetDatabase.GUIDToAssetPath(Guid);
            RecipeData recipe = AssetDatabase.LoadAssetAtPath<RecipeData>(AssetPath);

            if (recipe != null && recipe != NormalBread)
            {
                AllRecipes.Add(recipe);
            }
        }
        EditorUtility.SetDirty(this);

        Debug.Log($"총 {AllRecipes.Count}개의 레시피를 불러왔습니다.");
    }
#endif
}
