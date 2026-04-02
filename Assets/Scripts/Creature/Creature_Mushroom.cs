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
    public float EmitDuration = 2.0f;
    public float StopDuration = 2.0f;
    public float WeaknessThreshold = 50.0f;
    public float WeaknessDuration = 3.0f;

    [Header("포자 범위")]
    public float MinSporeRadius = 2.0f;
    public float MaxSporeRadius = 6.0f;
    private float CurrentSporeRadius;

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
                if (distanceToPlayer <= CurrentSporeRadius)
                {
                    ApplySporeDebuffToPlayer();
                }
                if (StateTimer >= EmitDuration)
                {
                    ChangeState(MushroomState.Charging);
                }
                break;
            case MushroomState.Stopped:
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
            case MushroomState.Emitting:
                CurrentSporeRadius = Random.Range(MinSporeRadius, MaxSporeRadius);

                if (CreatureRenderer != null) CreatureRenderer.material.color = OriginalColor;
                if (SporeSmokeEffect != null) SporeSmokeEffect.SetActive(true);

                if (SporeExplosionEffect != null)
                {
                    Instantiate(SporeExplosionEffect, transform.position, Quaternion.identity);
                }
                break;

            case MushroomState.Stopped:
                if (SporeSmokeEffect != null) SporeSmokeEffect.SetActive(false);
                break;

            case MushroomState.Weakness:
                if (CreatureRenderer != null) CreatureRenderer.material.color = Color.green;
                if (SporeSmokeEffect != null) SporeSmokeEffect.SetActive(false);
                break;
        }
    }

    void OnMouseDown()
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

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectRadius);

        if (Application.isPlaying && CurrentState == MushroomState.Emitting)
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawSphere(transform.position, CurrentSporeRadius);
        }
    }
}
