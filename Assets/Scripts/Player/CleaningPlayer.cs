using UnityEngine;

public class CleaningPlayer : MonoBehaviour
{
    public enum PlayerTools
    {
        Hand,
        Rag,
        Broom,
    }
    public PlayerTools CurrentTool;

    [Header("행주 사운드")]
    public AudioClip RagWipeSound;

    void Start()
    {
        CurrentTool = PlayerTools.Hand;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeTool(PlayerTools.Hand);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeTool(PlayerTools.Rag);
        //if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeTool(PlayerTools.Broom);

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2.0f);

            RaycastHit[] hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits)
            {
                Debug.DrawLine(ray.origin, hit.point, Color.green, 2.0f);
                Debug.Log($"{hit.collider.gameObject.name}");

                Dirt TargetDirt = hit.collider.GetComponent<Dirt>();
                if (TargetDirt != null)
                {
                    if (SoundManager.Instance != null && RagWipeSound != null && CurrentTool == PlayerTools.Rag)
                    {
                        SoundManager.Instance.PlaySFX(RagWipeSound);
                    }
                    TargetDirt.CleanDirt(CurrentTool);
                    return;
                }

                CleanEventNPC inspector = hit.collider.GetComponent<CleanEventNPC>();
                if (inspector != null)
                {
                    if (CurrentTool == PlayerTools.Hand)
                    {
                        Debug.Log($"[CleaningPlayer] 위생관리원에게 말을 걸었습니다.");
                        inspector.StartInspection();
                    }
                    else
                    {
                        Debug.Log($"[CleaningPlayer] 맨손으로 말을 걸어야 합니다.");
                    }
                }
            }
        }
    }

    public void ChangeTool(PlayerTools tools)
    {
        CurrentTool = tools;
        Debug.Log($"{CurrentTool}로 변경함");
    }


}
