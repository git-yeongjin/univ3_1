using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class NightEvent : MonoBehaviour
{
    [Header("씬 이름")]
    public string NightSceneName = "NightEventScene";

    [Header("밤 시간 및 진행 상태")]
    //밤 시간
    public float MaxNightTime = 300f;
    public float CurrentNightTime = 0f;
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
    }

    void Update()
    {
        //테스트
        if (Input.GetKeyDown(KeyCode.L))
        {
            GameManager.Instance.IncreaseCustomer();
        }

        if (CurrentDayCount > 0 && !NightEventFin)
        {
            CurrentNightTime -= Time.deltaTime;

            if (CurrentNightTime <= 0f)
            {
                CurrentNightTime = 0f;
                TimeOutNightEvent();
            }
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == NightSceneName)
        {
            StartNightEvent();
        }
    }

    public void StartNightEvent()
    {
        CurrentDayCount = GameManager.Instance.DayCount;

        CurrentNightTime = MaxNightTime;
        NightEventFin = false;

        Debug.Log($"[NightEvent] {CurrentDayCount}일차 밤이 시작되었습니다.");

        if (TutorialCreature == null)
        {
            TutorialCreature = FindInactiveTutorialCreature("TutorialCreature");

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

    private void TimeOutNightEvent()
    {
        if (NightEventFin) return;

        NightEventFin = true;
        Debug.Log("[NightEvent] 밤 사냥 제한 시간이 종료되었습니다!");

        // UI 스크립트를 찾아서 결과창 열기
        NightEventUI nightEventUI = FindAnyObjectByType<NightEventUI>();
        if (nightEventUI != null)
        {
            nightEventUI.ShowNightResult();
        }
    }

    private GameObject FindInactiveTutorialCreature(string objName)
    {
        //씬 정보 가져오기
        Scene nightScene = SceneManager.GetSceneByName(NightSceneName);
        if (!nightScene.isLoaded) return null;

        //해당 씬에 최상위 부모 오브젝트 가져오기
        GameObject[] rootObjects = nightScene.GetRootGameObjects();

        //자식까지 뒤지기
        foreach (GameObject rootObj in rootObjects)
        {
            Transform[] allChildren = rootObj.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {
                if (child.name == objName)
                {
                    return child.gameObject;
                }
            }
        }
        return null;
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
