using System.Collections.Generic;
using UnityEngine;

public class Dough : MonoBehaviour
{
    private GameManager GM;

    public RecipeData recipe;

    [Header("반죽에 들어간 재료")]
    public List<string> BreadMaterial = new List<string>();

    void Start()
    {
        GM = FindAnyObjectByType<GameManager>();
        if (GM == null) Debug.LogError($"[Dough] GameManager를 찾을 수 없습니다.");
    }

    public void AddMaterial(string BreadMaterialName)
    {
        if (!GM.isBakingTime)
        {
            Debug.Log("섞기가 완료된 반죽입니다.");
            return;
        }

        BreadMaterial.Add(BreadMaterialName);
        Debug.Log($"반죽에 {BreadMaterialName}을 추가했습니다. 총 재료 {BreadMaterial.Count}개 들어감");
    }

    public void ClearBreadMaterial()
    {
        Debug.Log("반죽에 들어간 재료가 초기화 되었습니다.");

        BreadMaterial.Clear();
    }

    public void FindRecipe()
    {
        Debug.Log("레시피 찾는 중");
        recipe = RecipeManager.Instance.FindRecipe(BreadMaterial);
    }
}
