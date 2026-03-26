using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Oven : MonoBehaviour
{
    //빵 굽는거 확인
    private bool isBaking = false;
    private float CurrentBakeTime = 0f;
    private RecipeData CurrentRecipe;
    public GameObject OvenUI;
    public TMP_Text CurrentBakeText;
    public GameObject OpenSceneBT;

    [SerializeField]
    private GameManager GM;

    void Start()
    {
        GM = FindAnyObjectByType<GameManager>();
        if (GM == null)
        {
            Debug.LogError("GameManager을 찾지 못했습니다.");
        }
        OvenUI.SetActive(false);
        OpenSceneBT.SetActive(false);
    }
    void Update()
    {
        if (isBaking)
        {
            CurrentBakeTime += Time.deltaTime;
            CurrentBakeText.text = $"{CurrentBakeTime:F1}";

            if (CurrentBakeTime >= CurrentRecipe.PerfectBakeTime)
            {
                CurrentBakeText.text = $"굽기 완료";
                OpenSceneBT.SetActive(true);
                isBaking = false;
            }
        }
    }

    public void StartBaking(RecipeData recipe)
    {
        CurrentRecipe = recipe;
        CurrentBakeTime = 0f;
        isBaking = true;

        OvenUI.SetActive(true);
        OpenSceneBT.SetActive(false);
        Debug.Log($"{CurrentRecipe.BreadName} 굽기 시작");

        switch (CurrentRecipe.Result)
        {
            case ResultBread.DollCake:
                if (!GM.DollCake) GM.DollCake = true;
                Debug.Log($"{CurrentRecipe.Result}를 판매할 수 있습니다.");
                break;
            case ResultBread.MushroomMuffin:
                if (!GM.DollCake) GM.MushroomMuffin = true;
                Debug.Log($"{CurrentRecipe.Result}를 판매할 수 있습니다.");
                break;
            case ResultBread.SlimePudding:
                if (!GM.DollCake) GM.SlimePudding = true;
                Debug.Log($"{CurrentRecipe.Result}를 판매할 수 있습니다.");
                break;
        }
    }

    public void EndBaking()
    {
        Debug.Log($"{CurrentRecipe.Result}제작 완료");
        OvenUI.SetActive(false);
    }
}
