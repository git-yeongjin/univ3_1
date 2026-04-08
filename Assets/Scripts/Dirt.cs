using Unity.VisualScripting;
using UnityEngine;

public class Dirt : MonoBehaviour
{
    public enum DirtType
    {
        none,
        Trash,
        Stain,
        creature1,
        creature2,
    }

    [Header("기본 설정")]
    public DirtType dirtType;
    public string DirtName;
    public bool isCreatureTrace;
    public CleaningPlayer.PlayerTools RequiredTool;
    public int HP = 0;

    bool trace;
    private CleanEvent cleanEvent;

    void Start()
    {
        cleanEvent = FindAnyObjectByType<CleanEvent>();
        if (cleanEvent == null) Debug.LogError($"[Dirt] CleanEvent스크립트를 찾을 수 없습니다.");
    }

    void OnValidate()
    {
        SetDirt();
    }

    public void CleanDirt(CleaningPlayer.PlayerTools tools)
    {
        Debug.Log($"플레이어 도구 : {tools}, 필요한 도구 : {RequiredTool}");
        if (tools == RequiredTool)
        {
            HP--;
            Debug.Log($"{DirtName}청소 중 남은 횟수 : {HP}");

            if (HP <= 0)
            {
                CleanUP();
            }
        }
    }

    private void CleanUP()
    {
        Debug.Log($"{DirtName}청소 완료");
        cleanEvent.IncreaseCount();

        Destroy(gameObject);
    }

    private void SetDirt()
    {
        switch (dirtType)
        {
            case DirtType.Trash:
                DirtName = "비닐 쓰레기";
                isCreatureTrace = false;
                RequiredTool = CleaningPlayer.PlayerTools.Hand;
                HP = 1;
                break;
            case DirtType.Stain:
                DirtName = "얼룩";
                isCreatureTrace = false;
                RequiredTool = CleaningPlayer.PlayerTools.Rag;
                HP = 5;
                break;
            case DirtType.creature1:
                DirtName = "크리쳐 얼룩";
                isCreatureTrace = true;
                RequiredTool = CleaningPlayer.PlayerTools.Rag;
                HP = 5;
                break;
            case DirtType.creature2:
                DirtName = "크리쳐 털";
                isCreatureTrace = true;
                RequiredTool = CleaningPlayer.PlayerTools.Hand;
                HP = 2;
                break;
        }
    }

    private bool CreatureTrace()
    {
        if (isCreatureTrace) trace = true;

        return trace;
    }
}
