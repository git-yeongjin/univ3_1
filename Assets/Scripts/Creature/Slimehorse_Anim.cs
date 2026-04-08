using System.Collections;
using UnityEngine;

public class Slimehorse_Anim : MonoBehaviour
{
    private Animator anim;
    private Creature_SlimeHorse horse;

    void Start()
    {
        anim = GetComponent<Animator>();
        horse = GetComponent<Creature_SlimeHorse>();

        StartCoroutine(RandomIdleRoutine());
    }

    void Update()
    {
        if (horse == null || anim == null) return;

        switch (horse.CurrentState)
        {
            case Creature_SlimeHorse.SlimeHorseState.Idle:
                anim.SetInteger("HorseState", 0);
                break;
            case Creature_SlimeHorse.SlimeHorseState.Stare:
                anim.SetInteger("HorseState", 0);
                break;
            case Creature_SlimeHorse.SlimeHorseState.Alert:
                break;
            case Creature_SlimeHorse.SlimeHorseState.Capturable:
                anim.SetInteger("HorseState", 0);
                break;
            case Creature_SlimeHorse.SlimeHorseState.Explore:
                anim.SetInteger("HorseState", 1);
                break;
            case Creature_SlimeHorse.SlimeHorseState.Attack:
                anim.SetInteger("HorseState", 2);
                break;
            case Creature_SlimeHorse.SlimeHorseState.Down:
                anim.SetInteger("HorseState", 3);
                break;
        }
    }

    private IEnumerator RandomIdleRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(3f, 6f);
            yield return new WaitForSeconds(waitTime);

            anim.SetInteger("IdleState", 1);

            yield return new WaitForSeconds(2f);

            anim.SetInteger("IdleState", 0);
        }
    }
}
