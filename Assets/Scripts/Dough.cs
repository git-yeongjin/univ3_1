using System.Collections.Generic;
using UnityEngine;

public class Dough : MonoBehaviour
{
    public RecipeData recipe;

    [Header("반죽에 들어간 재료")]
    public List<string> BreadMaterial = new List<string>();

    [Header("반죽 상태")]
    public bool isReadyToBake = false;

    public Mold MoldName;

    public void AddMaterial(string BreadMaterialName)
    {
        if (isReadyToBake)
        {
            Debug.Log("섞기가 완료된 반죽입니다.");
            return;
        }

        BreadMaterial.Add(BreadMaterialName);
        Debug.Log($"반죽에 {BreadMaterialName}을 추가했습니다. 총 재료 {BreadMaterial.Count}개 들어감");
    }

    public void AddMold(Mold mold)
    {
        if (!isReadyToBake)
        {
            Debug.Log("먼저 반죽을 섞어야 합니다.");
            return;
        }
        MoldName = mold;
        Debug.Log($"반죽을 {MoldName}에 넣었습니다.");
    }

    public void FindRecipe()
    {
        Debug.Log("레시피 찾는 중");
        recipe = RecipeManager.Instance.FindRecipe(BreadMaterial, MoldName);
    }
}
