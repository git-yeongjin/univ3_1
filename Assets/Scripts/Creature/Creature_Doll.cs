using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum DollItemType
{
    Button,
    Ribbon
}

public class Creature_Doll : MonoBehaviour
{
    private Creature BaseCreature;
    private GameManager GM;
    private Camera MainCamera;

    [Header("인형")]
    //현재 호감도
    public float CurrentAffection;
    //목표 호감도
    public float MaxAffection;
    //현재 불안도
    public float CurrentAnxiety;
    //최대 불안도 -> 넘기면 포획 실패
    public float MaxAnxiety = 100f;

    [Header("미니게임 상태")]
    public bool isGameStarted = false;
    public bool isCapturable = false;
    public float TimeLimit = 180f;
    [SerializeField] private float CurrentTime;
    private bool isHintShown = false;

    [Header("물건 설정")]
    public int RequiredItemCount = 0;
    public int CurrentDeliverd = 0;

    //현재 요구 아이템
    public DollItemType RequiredItem;
    //버튼
    public GameObject ButtonPrefab;
    //리본
    public GameObject RibbonPrefab;

    public LayerMask GroundLayer = ~0;
    public float ItemYOffset = 0.1f;

    [SerializeField]
    //랜덤 스폰 위치
    private Transform[] SpawnLocations;
    //스폰된 아이템 추적
    public List<GameObject> SpawnedItems = new List<GameObject>();

    [Header("UI 및 이펙트 프리팹")]
    public GameObject ExclamationMark;
    public GameObject HeartEffect;

    public GameObject PopupUI;
    public GameObject SpeechBubble;

    public Sprite[] SpeechBubbleSprites;

    void Start()
    {
        BaseCreature = GetComponent<Creature>();
        GM = FindAnyObjectByType<GameManager>();

        MainCamera = Camera.main;

        if (ExclamationMark != null) ExclamationMark.SetActive(false);
        //if (SpeechBubble != null) SpeechBubble.SetActive(false);
        if (HeartEffect != null) HeartEffect.SetActive(false);

    }

    public void SetItemSpawnLocations(Transform[] points)
    {
        SpawnLocations = points;
    }

    void Update()
    {
        if (isGameStarted)
        {
            PlayMiniGameTimer();
        }

        if (SpeechBubble != null && SpeechBubble.activeSelf)
        {
            SpeechBubble.transform.rotation = MainCamera.transform.rotation;
        }
    }

    void OnMouseDown()
    {
        if (isCapturable)
        {
            Debug.Log($"[인형 패턴] 포획 성공!");
            if (HeartEffect != null) HeartEffect.SetActive(false);
            BaseCreature.Capture();
            return;
        }

        if (!isGameStarted)
        {
            StartMiniGame();
        }
        else
        {
            TryDeliverItem();
        }
    }

    private void StartMiniGame()
    {
        isGameStarted = true;
        CurrentTime = TimeLimit;
        isHintShown = false;
        CurrentDeliverd = 0;

        if (ExclamationMark != null) ExclamationMark.SetActive(true);

        RequiredItemCount = 3;
        if (SpawnLocations != null && SpawnLocations.Length > 0)
        {
            RequiredItemCount = Mathf.Min(RequiredItemCount, SpawnLocations.Length);
        }


        RequiredItem = (Random.Range(0, 2) == 0) ? DollItemType.Button : DollItemType.Ribbon;
        string itemName = RequiredItem == DollItemType.Button ? "단추" : "리본";
        Debug.Log($"[인형 패턴] 찾아야 할 물건 개수 : {RequiredItemCount}개");

        if (SpeechBubble != null)
        {
            SpriteRenderer speechBubbleRenderer = SpeechBubble.GetComponent<SpriteRenderer>();
            switch (RequiredItem)
            {
                case DollItemType.Button:
                    speechBubbleRenderer.sprite = SpeechBubbleSprites[1];
                    break;
                case DollItemType.Ribbon:
                    speechBubbleRenderer.sprite = SpeechBubbleSprites[2];
                    break;
            }
        }

        SpawnHiddenItems();
    }

    private void PlayMiniGameTimer()
    {
        CurrentTime -= Time.deltaTime;
        CurrentAnxiety = Mathf.Lerp(MaxAnxiety, 0, CurrentTime / TimeLimit);

        if (CurrentTime <= 30f && !isHintShown)
        {
            ShowItemHints();
        }

        if (CurrentTime <= 0)
        {
            Debug.Log("[인형 패턴] 제한 시간 초과로 크리쳐가 도망갔습니다.");
            isGameStarted = false;

            CleanSpawnedItem();
            BaseCreature.Escape();
        }
    }

    private void TryDeliverItem()
    {
        PlayerInventory player = FindAnyObjectByType<PlayerInventory>();
        if (player == null) return;

        //임시
        bool playerHasItem = false;

        if (RequiredItem == DollItemType.Button && player.ButtonCount > 0)
        {
            player.ButtonCount--;
            playerHasItem = true;
        }
        else if (RequiredItem == DollItemType.Ribbon && player.RibbonCount > 0)
        {
            player.RibbonCount--;
            playerHasItem = true;
        }

        if (playerHasItem)
        {
            CurrentDeliverd++;
            Debug.Log($"[인형 패턴] 물건 전달 완료 ({CurrentDeliverd} / {RequiredItemCount})");

            if (CurrentDeliverd >= RequiredItemCount)
            {
                SuccessPattern();
            }
        }
    }

    private void SuccessPattern()
    {
        isGameStarted = false;
        isCapturable = true;

        CleanSpawnedItem();

        if (SpeechBubble != null) SpeechBubble.SetActive(false);
        if (HeartEffect != null) HeartEffect.SetActive(true);
    }

    private void SpawnHiddenItems()
    {
        if (SpawnLocations == null || SpawnLocations.Length < RequiredItemCount)
        {
            Debug.LogWarning("스폰 위치가 요구 개수보다 적습니다.");
            return;
        }

        GameObject prefabToSpawn = (RequiredItem == DollItemType.Button) ? ButtonPrefab : RibbonPrefab;

        List<Transform> availableLocations = new List<Transform>(SpawnLocations);

        for (int i = 0; i < RequiredItemCount; i++)
        {
            int randomIndex = Random.Range(0, availableLocations.Count);
            Transform spawnPoint = availableLocations[randomIndex];

            Vector3 finalSpawnPos = spawnPoint.position;
            RaycastHit hit;

            if (Physics.Raycast(spawnPoint.position + Vector3.up * 2f, Vector3.down, out hit, 20f, GroundLayer))
            {
                // 레이저가 땅에 부딪혔다면, 부딪힌 그 좌표(hit.point)를 가져오고 오프셋을 더합니다.
                finalSpawnPos = hit.point + (Vector3.up * ItemYOffset);
            }
            else
            {
                Debug.LogWarning($"[인형 패턴] {spawnPoint.name} 아래에 땅이 없어 원래 위치에 스폰합니다.");
            }

            GameObject newItem = Instantiate(prefabToSpawn, finalSpawnPos, spawnPoint.rotation);
            SpawnedItems.Add(newItem);

            availableLocations.RemoveAt(randomIndex);
        }

        Debug.Log($"[인형 패턴] 랜덤 위치에 아이템이 생성 되었습니다.");
    }

    private void CleanSpawnedItem()
    {
        foreach (GameObject item in SpawnedItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        SpawnedItems.Clear();
    }

    private void ShowItemHints()
    {
        isHintShown = true;
    }
}
