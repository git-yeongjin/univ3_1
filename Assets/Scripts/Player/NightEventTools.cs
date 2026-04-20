using System.Collections.Generic;
using UnityEngine;

public class NightEventTools : MonoBehaviour
{
    public enum NightTools
    {
        none,
        //GlassTube,
        //WaterGun,
        //BugNet,
        Ocarina
    }
    public NightTools Tools = NightTools.none;

    [Header("도구 3D 모델")]
    public GameObject OcarinaModel;

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

    [Header("사운드")]
    public AudioClip PlayOcarinaSound;

    void Start()
    {
        UpdateToolModels();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // 알파벳 위의 숫자 1
        {
            SwitchTool(NightTools.none);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // 알파벳 위의 숫자 2
        {
            SwitchTool(NightTools.Ocarina);
        }

        if (CurrentOcarinaTimer > 0)
        {
            CurrentOcarinaTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.F) && !isToolWindowOpen)
        {
            UseCurrentTool();
        }
    }

    private void SwitchTool(NightTools newTool)
    {
        if (Tools == newTool) return; // 이미 들고 있는 도구면 무시

        Tools = newTool;
        Debug.Log($"[도구 변경] 현재 장착 무기: {Tools}");

        UpdateToolModels();
    }

    private void UpdateToolModels()
    {
        if (OcarinaModel == null)
        {
            Debug.LogWarning("[NightEventTools] OcarinaModel이 인스펙터에 연결되지 않았습니다!");
            return;
        }

        if (Tools == NightTools.Ocarina)
        {
            OcarinaModel.SetActive(true);
        }
        else
        {
            OcarinaModel.SetActive(false);
        }
    }

    private void UseCurrentTool()
    {
        switch (Tools)
        {
            case NightTools.none:
                Debug.Log("[도구 사용] 맨손입니다.");
                break;

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

        if (SoundManager.Instance != null && PlayOcarinaSound != null)
        {
            SoundManager.Instance.PlaySFX(PlayOcarinaSound);
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
