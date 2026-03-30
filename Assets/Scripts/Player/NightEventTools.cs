using System.Collections.Generic;
using UnityEditor;
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

    [Header("오카리나")]
    public float OcarinaRange = 10.0f;
    public float OcarinaCoolDown = 5.0f;
    private float CurrentOcarinaTimer = 0f;

    void Start()
    {

    }


    void Update()
    {
        if (CurrentOcarinaTimer > 0)
        {
            CurrentOcarinaTimer -= Time.deltaTime;
        }

        if (Input.GetMouseButtonDown(0) && !isToolWindowOpen)
        {
            UseCurrentTool();
        }
    }

    private void UseCurrentTool()
    {
        switch (Tools)
        {
            case NightTools.Ocarina:
                PlayOcarina();
                break;
        }
    }

    private void PlayOcarina()
    {
        if (CurrentOcarinaTimer > 0)
        {
            Debug.Log($"[오카리나] 쿨타임 중입니다. {CurrentOcarinaTimer:F1}초 남음");
            return;
        }

        Debug.Log($"오카리나 연주 중");
        CurrentOcarinaTimer = OcarinaCoolDown;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, OcarinaRange);

        foreach (Collider hit in hitColliders)
        {
            Creature_SlimeHorse slimeHorse = hit.GetComponent<Creature_SlimeHorse>();
            if (slimeHorse != null)
            {
                slimeHorse.OnOcarinaused();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, OcarinaRange);
    }
}
