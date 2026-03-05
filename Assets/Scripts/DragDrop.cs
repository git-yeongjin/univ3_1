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
    private const string MoveObjTAG = "Moveable";

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


#if UNITY_EDITOR
        // 캐릭터 Ray 확인용 
        if (MoveObj != null)
        {
            Ray ray = new Ray(MoveObj.transform.position, Camera.main.transform.forward);
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);
        }
        Vector3 TouchPos = new Vector3(0, 0, 0);
#if UNITY_EDITOR
        TouchPos = Input.mousePosition;
#endif
        Ray test_ray = Camera.main.ScreenPointToRay(TouchPos);

        Debug.DrawRay(test_ray.origin, test_ray.direction * 1000, Color.blue);
#endif
    }


    private void TouchBeganEvent()
    {
        MoveObj = OnClickObjTag(MoveObjTAG);
        if (MoveObj != null)
        {
            BeforePosition = MoveObj.transform.position;
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
        if (MoveObj != null)
        {
            Collider moveObjCollider = MoveObj.GetComponent<Collider>();
            if (moveObjCollider != null)
            {
                moveObjCollider.enabled = false;
            }

            Ray ray = new Ray(MoveObj.transform.position, Camera.main.transform.forward);
            RaycastHit HitInfo;

            if (Physics.Raycast(ray, out HitInfo))
            {
                Debug.Log("hit info : " + HitInfo.collider.gameObject.name);

                Vector3 TargetPos = HitInfo.collider.gameObject.transform.position + Offset;
                TargetPos.z = BeforePosition.z;

                MoveObj.transform.position = TargetPos;
            }
            else
            {
                MoveObj.transform.position = BeforePosition;
            }

            if (moveObjCollider != null)
            {
                moveObjCollider.enabled = true;
            }

            MoveObj = null;
        }
    }

    //클릭 했을때 오브젝트 태그 확인
    private GameObject OnClickObjTag(string tag)
    {
        Vector3 touchPos = new Vector3(0, 0, 0);

        touchPos = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(touchPos);
        RaycastHit HitInfo;
        Physics.Raycast(ray, out HitInfo);

        if (HitInfo.collider != null)
        {
            GameObject hitObject = HitInfo.collider.gameObject;
            if (hitObject != null)
            {
                if (hitObject.gameObject.tag.Equals(tag))
                {
                    return hitObject;
                }
            }
        }

        return null;
    }
}
