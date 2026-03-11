using System.Collections.Generic;
using UnityEngine;

public class NightEventTools : MonoBehaviour
{
    public enum NightTools
    {
        none,
        GlassTube,
        WaterGun,
        BugNet,
        Ocarina
    }
    public NightTools Tools;

    [Header("도구 선택 창")]
    public bool isToolWindowOpen;
    public int MaxToolSlots = 4;
    public GameObject CurrentTool;

    [Header("크리쳐 인벤토리")]
    public List<Creature> CreatureInventory;
    public int MaxInventorySlots = 10;

    [Header("물총")]
    //기본 명중률
    public float BaseHitChance = 0.5f;
    //거리에 따른 추가 명중률
    public float DistanceBonusHitChance;
    //지속 시간
    public float FireDuration = 2.5f;
    //못맞출시 쿨타임
    public float MissCoolDown = 30.0f;
    //쏘는 중인지 여부
    public bool isFire;
    //쿨타임 계산용
    public float CurrentCoolDownTimer;

    void Start()
    {

    }


    void Update()
    {

    }
}
