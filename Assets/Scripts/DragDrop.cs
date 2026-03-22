using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop : MonoBehaviour
{
    private Vector2 LastTouchPos = Vector2.zero;
    private Vector2 CurrentTouchPos = Vector2.zero;

    private Vector3 PrePos = Vector3.zero;
    private Vector3 BeforePosition;
    private Vector3 Offset = new Vector3(0.0f, 0.0f, 0.0f);

    [SerializeField]
    private GameObject MoveObj;
    private string[] MoveObjTAG = { "Moveable", "Dough", "BreadMaterial", "Mold", "FinishedBread" };

    void Update()
    {
        LastTouchPos = CurrentTouchPos;
        CurrentTouchPos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            TouchBeganEvent();
            PrePos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            if (Input.mousePosition != PrePos)
            {
                TouchMovedEvent();
            }
            else
            {
                TouchStayEvent();
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            TouchEndedEvent();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ShowCaseSetting();
        }


#if UNITY_EDITOR
        // 캐릭터 Ray 확인용 
        if (MoveObj != null)
        {
            Ray ray = new Ray(MoveObj.transform.position, Camera.main.transform.forward);
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);
        }
        Vector3 TouchPos = new Vector3(0, 0, 0);

        TouchPos = Input.mousePosition;

        Ray test_ray = Camera.main.ScreenPointToRay(TouchPos);

        Debug.DrawRay(test_ray.origin, test_ray.direction * 1000, Color.blue);
#endif
    }

    private void ShowCaseSetting()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit HitInfo;

        if (Physics.Raycast(ray, out HitInfo))
        {
            Debug.Log("hit info : " + HitInfo.collider.gameObject.name);

            ShowCase showCase = HitInfo.collider.GetComponent<ShowCase>();

            if (showCase != null)
            {
                showCase.DisplayBread();
            }
        }
    }


    private void TouchBeganEvent()
    {
        GameObject clickObj = OnClickObjTag(MoveObjTAG);

        if (clickObj != null)
        {
            FinishedBread breadInfo = clickObj.GetComponent<FinishedBread>();

            if (breadInfo != null)
            {
                MoveObj = Instantiate(clickObj, clickObj.transform.position, clickObj.transform.rotation);
                MoveObj.name = clickObj.name + "_copy";
                MoveObj.GetComponent<FinishedBread>().MyBreadType = breadInfo.MyBreadType;

                Debug.Log($"진열대의 [{clickObj.name}]을 가져왔습니다.");
            }
            else
            {
                MoveObj = clickObj;
            }

            BeforePosition = MoveObj.transform.position;
            Collider moveObjCollider = MoveObj.GetComponent<Collider>();
            if (moveObjCollider != null)
            {
                moveObjCollider.enabled = false;
            }
        }
    }
    private void TouchMovedEvent()
    {
        if (MoveObj != null)
        {
            Vector3 TouchPos = Vector3.zero;

            TouchPos = Input.mousePosition;
            float MoveObj_Z = Camera.main.WorldToScreenPoint(MoveObj.transform.position).z;
            Vector3 WorldPos = Camera.main.ScreenToWorldPoint(new Vector3(TouchPos.x, TouchPos.y, MoveObj_Z));

            MoveObj.transform.position = Vector3.MoveTowards(MoveObj.transform.position, WorldPos, Time.deltaTime * 20f);
        }
    }
    private void TouchStayEvent()
    {
    }
    private void TouchEndedEvent()
    {
        if (MoveObj == null) return;


        Collider moveObjCollider = MoveObj.GetComponent<Collider>();
        if (moveObjCollider != null) moveObjCollider.enabled = true;

        Ray ray = new Ray(MoveObj.transform.position, Camera.main.transform.forward);
        RaycastHit HitInfo;

        if (Physics.Raycast(ray, out HitInfo))
        {
            Debug.Log("hit info : " + HitInfo.collider.gameObject.name);

            Oven targetOven = HitInfo.collider.GetComponent<Oven>();
            Dough targetDough = HitInfo.collider.GetComponent<Dough>();
            PackagingStation targetStation = HitInfo.collider.GetComponent<PackagingStation>();

            //반죽을 오븐에 넣을 때
            if (targetOven != null && MoveObj.CompareTag("Dough"))
            {
                Dough currentDough = MoveObj.GetComponent<Dough>();
                if (currentDough != null)
                {
                    if (currentDough.isMold)
                    {
                        currentDough.FindRecipe();
                        targetOven.StartBaking(currentDough.recipe);

                        //반죽을 오븐에 넣어서 반죽오브젝트 삭제
                        Destroy(MoveObj);
                        MoveObj = null;
                        return;
                    }
                    else
                    {
                        Debug.Log("반죽을 섞고 틀에 넣어야 합니다.");
                    }
                }
                else
                {
                    Debug.LogError($"오븐에 넣은 오브젝트에 Dough 스크립트가 없습니다.");
                }
            }

            //재료를 반죽에 넣을 때
            else if (targetDough != null && MoveObj.CompareTag("BreadMaterial"))
            {
                BreadMaterial breadMaterial = MoveObj.GetComponent<BreadMaterial>();
                if (breadMaterial != null)
                {
                    targetDough.AddMaterial(breadMaterial.GetMaterialName());
                }
            }

            //틀안에 반죽을 넣을 때
            else if (targetDough != null && MoveObj.CompareTag("Mold"))
            {
                MoldType moldType = MoveObj.GetComponent<MoldType>();
                if (moldType != null)
                {
                    targetDough.AddMold(moldType.GetMoldName());
                }
            }

            //빵을 포장대에 놓을 때
            else if (targetStation != null && MoveObj.CompareTag("FinishedBread"))
            {
                FinishedBread bread = MoveObj.GetComponent<FinishedBread>();
                if (bread != null)
                {
                    targetStation.AskPackaging(bread);
                    MoveObj = null;
                    return;
                }
            }
        }

        if (MoveObj.CompareTag("FinishedBread"))
        {
            Destroy(MoveObj);
        }
        else
        {
            MoveObj.transform.position = BeforePosition;
        }
        MoveObj = null;
    }

    //클릭 했을때 오브젝트 태그 확인
    private GameObject OnClickObjTag(string[] targetTags)
    {
        Vector3 touchPos = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(touchPos);
        RaycastHit HitInfo;
        Physics.Raycast(ray, out HitInfo);

        if (HitInfo.collider != null)
        {
            GameObject hitObject = HitInfo.collider.gameObject;

            foreach (string tag in targetTags)
            {
                if (hitObject.CompareTag(tag))
                {
                    return hitObject;
                }
            }
        }

        return null;
    }
}
