using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class NightEvent : MonoBehaviour
{
    [Header("밤 시간 및 진행 상태")]
    //밤 시간
    public int NightTime = 0;
    public int CurrentDayCount;
    //밤 이벤트 종료
    public bool NightEventFin = false;

    [Header("크리쳐 포획 정보")]
    public int CurrentCatchCount = 0;
    public int MaxCatchCount = 10;
    public bool isCreatureUnlockedToday = false;

    [Header("크리쳐 데이터 목록")]
    public List<CreatureData> AllCreatureDatas = new List<CreatureData>();

    public GameObject TutorialCreature;

    void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError($"[NightEvent] GameManager.Instance가 존재하지 않습니다.");
            return;
        }
        StartNightEvent();
    }

    void Update()
    {
        //테스트
        if (Input.GetKeyDown(KeyCode.L))
        {
            GameManager.Instance.IncreaseCustomer();
        }
    }

    public void StartNightEvent()
    {
        CurrentDayCount = GameManager.Instance.DayCount;

        Debug.Log($"[NightEvent] {CurrentDayCount}일차 밤이 시작되었습니다.");

        if (TutorialCreature == null)
        {
            TutorialCreature = GameObject.Find("TutorialCreature");
            if (TutorialCreature == null && CurrentDayCount == 0)
            {
                Debug.LogError("[NightEvent] 'TutorialCreature'를 찾을 수 없습니다 하이어라키 이름과 Active 상태를 확인해주세요.");
                return;
            }
        }

        if (CurrentDayCount == 0)
        {
            Debug.Log("[NightEvent] 0일차 튜토리얼 크리쳐를 생성합니다.");

            if (TutorialCreature != null) TutorialCreature.SetActive(true);
        }
        else
        {
            if (TutorialCreature != null) TutorialCreature.SetActive(false);
            CheckUnlockCreature();
        }
    }

    private void CheckUnlockCreature()
    {
        if (AllCreatureDatas == null || AllCreatureDatas.Count == 0)
        {
            Debug.LogWarning("[NightEvent] 크리쳐 데이터가 등록되지 않았습니다.");
            return;
        }
        isCreatureUnlockedToday = false;

        foreach (CreatureData creature in AllCreatureDatas)
        {
            //오늘 날차에 해금되는 크리쳐인지 검사
            if (creature.UnLockDay == CurrentDayCount)
            {
                Debug.Log($"{creature.name}을 포획할 수 있습니다");
                isCreatureUnlockedToday = true;
            }
        }
    }


#if UNITY_EDITOR
    [ContextMenu("모든 크리쳐 불러오기")]
    public void LoadAllRecipes()
    {
        AllCreatureDatas.Clear();

        string[] Guids = AssetDatabase.FindAssets("t:CreatureData");

        foreach (string Guid in Guids)
        {
            string AssetPath = AssetDatabase.GUIDToAssetPath(Guid);
            CreatureData creatureData = AssetDatabase.LoadAssetAtPath<CreatureData>(AssetPath);

            if (creatureData != null)
            {
                AllCreatureDatas.Add(creatureData);
            }
        }
        EditorUtility.SetDirty(this);

        Debug.Log($"총 {AllCreatureDatas.Count}개의 크리쳐를 불러왔습니다.");
    }
#endif
}
