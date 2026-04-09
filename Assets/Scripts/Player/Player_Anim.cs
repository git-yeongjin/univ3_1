using UnityEngine;

public class Player_Anim : MonoBehaviour
{
    private BakePlayer bakePlayer;
    private Player player;
    private NightEventPlayer nightEventPlayer;

    private Animator anim;

    private Vector3 LastPosition;

    void Start()
    {
        bakePlayer = GetComponent<BakePlayer>();
        player = GetComponent<Player>();
        nightEventPlayer = GetComponent<NightEventPlayer>();

        anim = GetComponent<Animator>();
        LastPosition = transform.position;
    }

    void Update()
    {
        float moveDistance = Vector3.Distance(transform.position, LastPosition);

        bool isWalking = moveDistance > 0.001f;

        if (anim != null)
        {
            anim.SetBool("isWalk", isWalking);
        }

        LastPosition = transform.position;
    }
}
