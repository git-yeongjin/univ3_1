using System.Collections;
using UnityEngine;

public class Mushroom_Anim : MonoBehaviour
{
    private Animator anim;
    private Creature_Mushroom mush;

    void Start()
    {
        anim = GetComponent<Animator>();
        mush = GetComponent<Creature_Mushroom>();

        StartCoroutine(RandomIdleRoutine());
    }

    void Update()
    {
        if (mush == null || anim == null) return;

        switch (mush.CurrentState)
        {
            case Creature_Mushroom.MushroomState.Idle:
                anim.SetInteger("MushState", 0);
                break;
            case Creature_Mushroom.MushroomState.Charging:
                anim.SetInteger("MushState", 1);
                break;
            case Creature_Mushroom.MushroomState.Emitting:
                anim.SetInteger("MushState", 2);
                break;
            case Creature_Mushroom.MushroomState.Stopped:
                break;
            case Creature_Mushroom.MushroomState.Weakness:
                anim.SetInteger("MushState", 0);
                break;
        }
    }

    private IEnumerator RandomIdleRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(3f, 6f);
            yield return new WaitForSeconds(waitTime);

            int randomAction = Random.Range(1, 3);
            anim.SetInteger("IdleState", randomAction);

            yield return new WaitForSeconds(2f);

            anim.SetInteger("IdleState", 0);
        }
    }
}
