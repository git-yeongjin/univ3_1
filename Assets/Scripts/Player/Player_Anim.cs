using UnityEngine;

public class Player_Anim : MonoBehaviour
{
    private BakePlayer bakePlayer;
    private Player player;
    private NightEventPlayer nightEventPlayer;
    private NightEventTools nightTools;

    private Animator anim;

    private Vector3 LastPosition;

    void Start()
    {
        bakePlayer = GetComponent<BakePlayer>();
        player = GetComponent<Player>();
        nightEventPlayer = GetComponent<NightEventPlayer>();
        nightTools = GetComponent<NightEventTools>();

        anim = GetComponent<Animator>();
        LastPosition = transform.position;
    }

    void Update()
    {
        float moveDistance = Vector3.Distance(transform.position, LastPosition);

        bool isWalking = moveDistance > 0.001f;

        bool isHoldingOcarina = false;
        if (nightTools != null && nightTools.Tools == NightEventTools.NightTools.Ocarina)
        {
            isHoldingOcarina = true;
        }

        if (anim != null)
        {
            anim.SetBool("isWalk", isWalking);

            anim.SetBool("isOcarina", isHoldingOcarina);
        }

        LastPosition = transform.position;
    }
}
