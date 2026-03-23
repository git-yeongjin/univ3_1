using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class NightEvent : MonoBehaviour
{
    private GameManager GM;

    [Header("밤 이벤트")]
    //밤 시간
    public int NightTime = 0;
    public int CurrentDayCount;

    public int CurrentCatchCount = 0;
    public int MaxCatchCount = 10;
    //밤 이벤트 종료
    public bool NightEventFin = false;

    public bool isCreatureUnlockedToday = false;

    [Header("크리쳐 데이터 목록")]
    public List<CreatureData> AllCreatureDatas = new List<CreatureData>();

    void Start()
    {
        GM = GetComponent<GameManager>();
        if (GM == null)
        {
            Debug.LogError($"GM을 못찾음");
            return;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            GM.IncreaseCustomer();
        }
    }

    public void StartNightEvent()
    {
        if (GM == null) GM = FindAnyObjectByType<GameManager>();
        CurrentDayCount = GM.DayCount;

        CheckUnlockCreature();
    }

    private void CheckUnlockCreature()
    {
        if (AllCreatureDatas == null || AllCreatureDatas.Count == 0)
        {
            Debug.LogWarning("NightEvent에 크리쳐 데이터가 등록되지 않았습니다.");
            return;
        }
        isCreatureUnlockedToday = false;

        foreach (CreatureData creature in AllCreatureDatas)
        {
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
