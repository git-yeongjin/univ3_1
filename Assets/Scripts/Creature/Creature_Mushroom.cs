using UnityEngine;
using System.Collections;

public class Creature_Mushroom : MonoBehaviour
{
    private Creature BaseCreature;
    private Transform PlayerTransform;
    private Animator anim;

    public enum MushroomState { Idle, Charging, Emitting, Stopped, Weakness }
    public MushroomState CurrentState = MushroomState.Idle;

    [Header("거리 및 시간 설정")]
    public float DetectRadius = 6.0f;
    public float DiscoverDuration = 2.0f;
    public float ChargeDuration = 2.0f;
    //public float EmitDuration = 4.0f;
    //포자가 퍼지는 속도
    public float SmokeExpandSpeed = 2.0f;
    public float StopDuration = 2.0f;
    public float WeaknessThreshold = 50.0f;
    public float WeaknessDuration = 3.0f;

    [Header("포자 범위")]
    public float MinSporeRadius = 2.0f;
    public float MaxSporeRadius = 6.0f;

    private float CurrentSporeRadius;
    private float CurrentVisualRadius = 0f;

    [Header("이펙트 및 시작 요소")]
    public GameObject SporeSmokeEffect;
    public GameObject SporeExplosionEffect;
    public Renderer CreatureRenderer;
    private Color OriginalColor;

    private float StateTimer = 0f;
    private float TotalAwareTime = 0f;
    private int FailCount = 0;
    private bool isAware = false;

    void Start()
    {
        anim = GetComponent<Animator>();

        BaseCreature = GetComponent<Creature>();
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) PlayerTransform = player.transform;

        if (CreatureRenderer != null) OriginalColor = CreatureRenderer.material.color;

        if (SporeSmokeEffect != null) SporeSmokeEffect.SetActive(false);
    }

    void Update()
    {
        if (PlayerTransform == null) return;

        float DistanceToPlayer = Vector3.Distance(transform.position, PlayerTransform.position);

        if (!isAware && DistanceToPlayer <= DetectRadius)
        {
            StartPattern();
        }

        if (isAware)
        {
            TotalAwareTime += Time.deltaTime;

            if (TotalAwareTime >= WeaknessThreshold && CurrentState != MushroomState.Weakness)
            {
                TriggerWeakness();
            }
            else
            {
                HandleStateTimers(DistanceToPlayer);
            }
        }
    }

    private void TriggerWeakness()
    {
        TotalAwareTime = 0f;

        ChangeState(MushroomState.Weakness);
    }

    private void StartPattern()
    {
        isAware = true;
        TotalAwareTime = 0f;

        StartCoroutine(DiscoverRoutine());
    }

    private IEnumerator DiscoverRoutine()
    {
        anim.SetTrigger("OnDiscovered");

        yield return new WaitForSeconds(DiscoverDuration);

        ChangeState(MushroomState.Charging);
    }

    private void HandleStateTimers(float distanceToPlayer)
    {
        StateTimer += Time.deltaTime;

        switch (CurrentState)
        {
            case MushroomState.Charging:
                if (StateTimer >= ChargeDuration)
                {
                    ChangeState(MushroomState.Emitting);
                }
                break;

            case MushroomState.Emitting:
                CurrentVisualRadius += SmokeExpandSpeed * Time.deltaTime;

                if (SporeSmokeEffect != null)
                {
                    SporeSmokeEffect.transform.localScale = Vector3.one * (CurrentVisualRadius * 2f);
                }
                if (distanceToPlayer <= CurrentVisualRadius)
                {
                    ApplySporeDebuffToPlayer();
                }
                if (CurrentVisualRadius >= CurrentSporeRadius)
                {
                    ChangeState(MushroomState.Stopped);
                }
                break;

            case MushroomState.Stopped:
                if (distanceToPlayer <= CurrentSporeRadius)
                {
                    ApplySporeDebuffToPlayer();
                }

                if (StateTimer >= StopDuration)
                {
                    ChangeState(MushroomState.Charging);
                }
                break;
            case MushroomState.Weakness:
                if (StateTimer >= WeaknessDuration)
                {
                    ChangeState(MushroomState.Charging);
                }
                break;
        }
    }

    private void ChangeState(MushroomState newState)
    {
        CurrentState = newState;
        StateTimer = 0f;

        switch (CurrentState)
        {
            case MushroomState.Charging:
                //차징 상태일 때는 포자 이펙트 끄기
                if (SporeSmokeEffect != null) SporeSmokeEffect.SetActive(false);
                if (CreatureRenderer != null) CreatureRenderer.material.color = OriginalColor;
                break;

            case MushroomState.Emitting:
                CurrentSporeRadius = Random.Range(MinSporeRadius, MaxSporeRadius);
                CurrentVisualRadius = 0f;

                if (CreatureRenderer != null) CreatureRenderer.material.color = OriginalColor;
                if (SporeSmokeEffect != null)
                {
                    SporeSmokeEffect.transform.localScale = Vector3.zero;
                    SporeSmokeEffect.SetActive(true);

                    ParticleSystem[] particleSystems = SporeSmokeEffect.GetComponentsInChildren<ParticleSystem>();

                    foreach (ParticleSystem ps in particleSystems)
                    {
                        ps.Clear();
                        ps.Play();
                    }
                }

                if (SporeExplosionEffect != null)
                {
                    Instantiate(SporeExplosionEffect, transform.position, Quaternion.identity);
                }
                break;

            case MushroomState.Stopped:
                //if (SporeSmokeEffect != null) SporeSmokeEffect.SetActive(false);
                break;

            case MushroomState.Weakness:
                if (CreatureRenderer != null) CreatureRenderer.material.color = Color.green;
                if (SporeSmokeEffect != null) SporeSmokeEffect.SetActive(false);
                break;
        }
    }

    public void OnPlayerInteract()
    {
        if (!isAware) return;

        if (CurrentState == MushroomState.Stopped || CurrentState == MushroomState.Weakness)
        {
            BaseCreature.Capture();
        }
        else if (CurrentState == MushroomState.Emitting)
        {
            HandleCaptureFailure();
        }
    }

    private void HandleCaptureFailure()
    {
        FailCount++;

        if (FailCount == 1)
        {
            ChangeState(MushroomState.Emitting);
        }
        else if (FailCount >= 2)
        {
            BaseCreature.Escape();
        }
    }

    private void ApplySporeDebuffToPlayer()
    {
        if (SporeScreenEffect.Instance != null)
        {
            SporeScreenEffect.Instance.ApplyFog();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectRadius);

        if (Application.isPlaying && CurrentState == MushroomState.Emitting)
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawSphere(transform.position, CurrentVisualRadius);
        }
    }
}
