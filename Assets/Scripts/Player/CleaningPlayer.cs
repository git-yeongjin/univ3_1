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



    void Start()
    {
        CurrentTool = PlayerTools.Hand;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeTool(PlayerTools.Hand);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeTool(PlayerTools.Rag);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeTool(PlayerTools.Broom);

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log($"{hit.collider.gameObject.name}");
                Dirt TargetDirt = hit.collider.GetComponent<Dirt>();
                if (TargetDirt != null)
                {
                    TargetDirt.CleanDirt(CurrentTool);
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
