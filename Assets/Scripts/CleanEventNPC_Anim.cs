using UnityEngine;
using UnityEngine.AI;

public class CleanEventNPC_Anim : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (agent != null && anim != null)
        {
            bool isWalking = agent.velocity.magnitude > 0.1f;

            // 애니메이터에 걷기 상태 전달
            anim.SetBool("isWalk", isWalking);
        }
    }
}
