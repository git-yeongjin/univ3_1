using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class DragDrop : MonoBehaviour
{
    private Vector2 LastTouchPos = Vector2.zero;
    private Vector2 CurrentTouchPos = Vector2.zero;

    private Vector3 PrePos = Vector3.zero;
    private Vector3 BeforePosition;
    private Vector3 Offset = new Vector3(0.0f, 0.0f, 0.0f);

    private bool isHolding = false;

    [SerializeField]
    private GameObject MoveObj;
    private string[] MoveObjTAG = { "Moveable", "Dough", "BreadMaterial", "FinishedBread" };

    [Header("물건 들기 설정")]
    public Transform PlayerTransform;
    public Transform HoldPoint;

    public float MaxInteractDistance = 6.0f;

    private float holdZ;
    private Quaternion RotationOffset;
    private Quaternion InitialRotation;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isHolding) // 손이 비어있다면?
            {
                // 줍기를 시도해보고, 성공했으면 isHolding을 true로!
                if (TryPickUp())
                {
                    isHolding = true;
                }
                else
                {
                    // 줍기에 실패했다면(앞에 빵이 없다면) 진열대 기능인지 확인
                    ShowCaseSetting();
                }
            }
            else // 물건을 들고 있다면?
            {
                // 내려놓기 (또는 상호작용)
                TryDrop();
                isHolding = false; // 물건을 놨으니 다시 빈손으로!
            }
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

    void LateUpdate()
    {
        if (isHolding && MoveObj != null && HoldPoint != null)
        {
            MoveObj.transform.position = HoldPoint.position;
            MoveObj.transform.rotation = HoldPoint.rotation * RotationOffset;
        }
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

    private bool TryPickUp()
    {
        GameObject clickObj = OnClickObjTag(MoveObjTAG);

        if (clickObj != null)
        {
            if (clickObj.CompareTag("Dough") && GameManager.Instance.isBakingTime)
            {
                Debug.Log($"반죽을 먼저 섞어야 합니다.");
                return false;
            }
            BeforePosition = clickObj.transform.position;

            Vector3 holdPos = clickObj.transform.position;

            if (HoldPoint != null)
            {
                holdPos = HoldPoint.position;
            }
            FinishedBread breadInfo = clickObj.GetComponent<FinishedBread>();

            if (breadInfo != null)
            {
                MoveObj = Instantiate(clickObj, holdPos, clickObj.transform.rotation);
                MoveObj.name = clickObj.name + "_copy";
                MoveObj.GetComponent<FinishedBread>().MyBreadType = breadInfo.MyBreadType;

                Debug.Log($"진열대의 [{clickObj.name}]을 가져왔습니다.");
            }
            else
            {
                MoveObj = clickObj;
                MoveObj.transform.position = holdPos;
            }

            if (HoldPoint != null)
            {
                RotationOffset = Quaternion.Inverse(HoldPoint.rotation) * MoveObj.transform.rotation;
            }
            else
            {
                InitialRotation = MoveObj.transform.rotation;
            }

            holdZ = Camera.main.WorldToScreenPoint(holdPos).z;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, holdZ));

            Offset = holdPos - mouseWorldPos;

            Collider moveObjCollider = MoveObj.GetComponent<Collider>();
            if (moveObjCollider != null)
            {
                moveObjCollider.enabled = false;
            }

            return true;
        }
        return false;
    }
    /*
    private void TouchMovedEvent()
    {
        if (MoveObj != null)
        {
            if (HoldPoint != null)
            {
                MoveObj.transform.position = HoldPoint.position;
                MoveObj.transform.rotation = HoldPoint.rotation * RotationOffset;
            }
            else
            {
                Vector3 TouchPos = Input.mousePosition;
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(TouchPos.x, TouchPos.y, holdZ));
                MoveObj.transform.position = worldPos + Offset;
            }
        }
    }
    private void TouchStayEvent()
    {
        if (MoveObj != null && HoldPoint != null)
        {
            MoveObj.transform.position = HoldPoint.position;
            MoveObj.transform.rotation = HoldPoint.rotation * RotationOffset;
        }
    }
    */
    private void TryDrop()
    {
        if (MoveObj == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit HitInfo;

        if (Physics.Raycast(ray, out HitInfo))
        {
            Debug.Log("hit info : " + HitInfo.collider.gameObject.name);

            if (PlayerTransform != null)
            {
                Vector3 closestPoint = HitInfo.collider.ClosestPoint(PlayerTransform.position);
                float distance = Vector3.Distance(PlayerTransform.position, closestPoint);
                if (distance > MaxInteractDistance)
                {
                    Debug.Log("상호작용 할 수 있는 거리보다 멉니다.");
                    goto FAIL_INTERACT;
                }
            }

            Oven targetOven = HitInfo.collider.GetComponent<Oven>();
            Dough targetDough = HitInfo.collider.GetComponent<Dough>();
            PackagingStation targetStation = HitInfo.collider.GetComponent<PackagingStation>();

            //반죽을 오븐에 넣을 때
            if (targetOven != null && MoveObj.CompareTag("Dough"))
            {
                Dough currentDough = MoveObj.GetComponent<Dough>();
                if (currentDough != null)
                {
                    currentDough.FindRecipe();
                    if (currentDough.recipe == null) MoveObj.transform.position = BeforePosition;
                    else
                    {
                        targetOven.StartBaking(currentDough.recipe);
                        if (GameManager.Instance.isTutorial)
                        {
                            BakeEventUI bakeEventUI = FindAnyObjectByType<BakeEventUI>();
                            bakeEventUI.OpenCutSceneUI();
                        }
                        //반죽을 오븐에 넣어서 반죽오브젝트 삭제
                        Destroy(MoveObj);
                    }
                    MoveObj = null;
                    return;
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

            //빵을 포장대에 놓을 때
            else if (targetStation != null && MoveObj.CompareTag("FinishedBread"))
            {
                FinishedBread bread = MoveObj.GetComponent<FinishedBread>();
                if (bread != null)
                {
                    bool isSold = targetStation.AskPackaging(bread, HitInfo.collider.gameObject.name);
                    if (isSold)
                    {
                        Destroy(MoveObj);
                    }
                    else
                    {
                        if (MoveObj.name.Contains("_copy"))
                        {
                            Destroy(MoveObj);
                        }
                        else
                        {
                            MoveObj.transform.position = BeforePosition;
                        }
                    }

                    MoveObj = null;
                    return;
                }
            }
        }

    FAIL_INTERACT:

        if (MoveObj.CompareTag("FinishedBread"))
        {
            Destroy(MoveObj);
        }
        else
        {
            MoveObj.transform.position = BeforePosition;
        }

        if (MoveObj != null)
        {
            Collider moveObjCollider = MoveObj.GetComponent<Collider>();

            if (moveObjCollider != null) moveObjCollider.enabled = true;
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

            if (PlayerTransform != null)
            {
                Vector3 closestPoint = HitInfo.collider.ClosestPoint(PlayerTransform.position);
                float distance = Vector3.Distance(PlayerTransform.position, closestPoint);
                if (distance > MaxInteractDistance)
                {
                    Debug.Log("잡을 오브젝트가 멀리 있습니다.");
                    return null;
                }
            }

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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        if (PlayerTransform != null)
        {
            Gizmos.DrawWireSphere(PlayerTransform.position, MaxInteractDistance);
        }
    }
}
