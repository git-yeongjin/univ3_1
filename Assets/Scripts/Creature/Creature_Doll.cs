using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Creature_Doll : MonoBehaviour
{
    private Creature BaseCreature;
    private GameManager GM;

    [Header("인형")]
    //현재 호감도
    public float CurrentAffection;
    //목표 호감도
    public float MaxAffection;
    //현재 불안도
    public float CurrentAnxiety;
    //최대 불안도 -> 넘기면 포획 실패
    public float MaxAnxiety;

    [Header("미니게임 상태")]
    public bool isGameStarted = false;
    public float TimeLimit = 180f;
    private float CurrentTime;
    private bool isHintShown = false;

    [Header("물건 설정")]
    public int RequiredItemCount = 0;
    public int CurrentDeliverd = 0;

    [Header("UI 및 이펙트 프리팹")]
    public GameObject ExclamationMark;
    public GameObject HeartEffect;

    void Start()
    {
        BaseCreature = GetComponent<Creature>();
        GM = FindAnyObjectByType<GameManager>();

        if (ExclamationMark != null) ExclamationMark.SetActive(false);
    }

    void Update()
    {
        if (isGameStarted)
        {
            PlayMiniGameTimer();
        }
    }

    void OnMouseDown()
    {
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

        if (ExclamationMark != null) ExclamationMark.SetActive(true);

        if (GM != null && GM.DayCount <= 4) RequiredItemCount = 7;
        else RequiredItemCount = Random.Range(3, 5);

        Debug.Log($"[인형 패턴] 찾아야 할 물건 개수 : {RequiredItemCount}개");

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

            BaseCreature.Escape();
        }
    }

    private void TryDeliverItem()
    {
        //임시
        bool playerHasItem = true;

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

        if (HeartEffect != null) Instantiate(HeartEffect, transform.position, Quaternion.identity);

        BaseCreature.Capture();
    }

    private void SpawnHiddenItems()
    {

    }

    private void ShowItemHints()
    {
        isHintShown = true;
    }
}
