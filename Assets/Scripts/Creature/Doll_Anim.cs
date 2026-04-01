using System.Collections;
using UnityEngine;

public class Doll_Anim : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        StartCoroutine(RandomIdleRoutine());
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
