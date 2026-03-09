using UnityEngine;

public class Oven : MonoBehaviour
{
    //빵 굽는거 확인
    private bool isBaking = false;
    private float CurrentBakeTime = 0f;
    private RecipeData CurrentRecipe;

    [SerializeField]
    private DayEvent dayEvent;

    void Start()
    {
        if (dayEvent == null)
        {
            Debug.LogError($"DayEvent가 연결되지 않았습니다.");
        }
    }
    void Update()
    {
        if (isBaking)
        {
            CurrentBakeTime += Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndBaking();
        }
    }

    public void StartBaking(RecipeData recipe)
    {
        CurrentRecipe = recipe;
        CurrentBakeTime = 0f;
        isBaking = true;
        Debug.Log($"{CurrentRecipe.BreadName} 굽기 시작");
    }

    public void EndBaking()
    {
        if (!isBaking) return;
        isBaking = false;

        Debug.Log($"{CurrentBakeTime:F1}초 만에 빵을 꺼냈습니다.");
        if (CurrentBakeTime >= CurrentRecipe.PerfectBakeTime - CurrentRecipe.ErrorMargin &&
            CurrentBakeTime <= CurrentRecipe.PerfectBakeTime + CurrentRecipe.ErrorMargin)
        {
            if (dayEvent != null)
            {
                dayEvent.AddPerfectBread();
            }
            else
            {
                Debug.LogError($"DayEvent가 연결되지 않았습니다.");
            }

        }
        else
        {
            Debug.Log($"빵이 덜 익거나 탔습니다. 꺼낸 시간 : {CurrentBakeTime:F1}초 손님 수 증가 안됨");
        }
    }
}
