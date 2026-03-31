using TMPro;
using UnityEngine;

public class Oven : MonoBehaviour
{
    [Header("오븐 상태")]
    //빵 굽는거 확인
    private bool isBaking = false;
    private float CurrentBakeTime = 0f;
    private RecipeData CurrentRecipe;

    [Header("UI")]
    public GameObject OvenUI;
    public TMP_Text CurrentBakeText;
    public GameObject OpenSceneBT;

    void Start()
    {
        if (OvenUI != null) OvenUI.SetActive(false);
        if (OpenSceneBT != null) OpenSceneBT.SetActive(false);
    }
    void Update()
    {
        if (isBaking)
        {
            CurrentBakeTime += Time.deltaTime;
            if (CurrentBakeText != null) CurrentBakeText.text = $"{CurrentBakeTime:F1}";

            if (CurrentBakeTime >= CurrentRecipe.PerfectBakeTime)
            {
                if (CurrentBakeText != null) CurrentBakeText.text = $"굽기 완료";
                if (OpenSceneBT != null) OpenSceneBT.SetActive(true);

                isBaking = false;
                Debug.Log($"[Oven] {CurrentRecipe.BreadName} 굽기 완료");
            }
        }
    }

    public void StartBaking(RecipeData recipe)
    {
        CurrentRecipe = recipe;
        CurrentBakeTime = 0f;
        isBaking = true;

        if (OvenUI != null) OvenUI.SetActive(true);
        if (OpenSceneBT != null) OpenSceneBT.SetActive(false);
        Debug.Log($"[Oven] {CurrentRecipe.BreadName} 굽기 시작");

        switch (CurrentRecipe.Result)
        {
            case ResultBread.DollCake:
                GameManager.Instance.DollCake = true;
                Debug.Log($"[Oven] {CurrentRecipe.Result}를 판매할 수 있습니다.");
                break;
            case ResultBread.MushroomMuffin:
                GameManager.Instance.MushroomMuffin = true;
                Debug.Log($"[Oven] {CurrentRecipe.Result}를 판매할 수 있습니다.");
                break;
            case ResultBread.SlimePudding:
                GameManager.Instance.SlimePudding = true;
                Debug.Log($"[Oven] {CurrentRecipe.Result}를 판매할 수 있습니다.");
                break;
        }
    }

    public void EndBaking()
    {
        Debug.Log($"[Oven] {CurrentRecipe.Result}제작 완료");
        if (OvenUI != null) OvenUI.SetActive(false);
    }
}
