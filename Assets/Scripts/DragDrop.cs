using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop : MonoBehaviour
{

    private Vector2 LastTouchPos = Vector2.zero;
    private Vector2 CurrentTouchPos = Vector2.zero;

    private Vector3 PrePos = Vector3.zero;
    private Vector3 BeforePosition;
    private Vector3 Offset = new Vector3(0.0f, 1.0f, 0.0f);

    [SerializeField]
    private GameObject MoveObj;
    private const string MoveObjTAG = "Moveable";

    void Update()
    {
#if UNITY_EDITOR
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
#else
        if (Input.touchCount <= 0) return;

        Touch touchInfo = Input.GetTouch(0);
        LastTouchPos = CurrentTouchPos;
        CurrentTouchPos = touchInfo.position;

        switch (touchInfo.phase)
            {
                case TouchPhase.Began:
                    TouchBeganEvent();
                    break;

                case TouchPhase.Moved:
                    TouchMovedEvent();
                    break;

                case TouchPhase.Stationary:
                    TouchStayEvent();
                    break;

                case TouchPhase.Ended:
                    TouchEndedEvent();
                    break;
            }
#endif

#if UNITY_EDITOR
        // 캐릭터 Ray 확인용 
        if (MoveObj != null)
        {
            Ray ray = new Ray(MoveObj.transform.position, Camera.main.transform.forward);
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);
        }
    }
#endif

    private void TouchBeganEvent()
    {

    }
    private void TouchMovedEvent()
    {

    }
    private void TouchStayEvent()
    {

    }
    private void TouchEndedEvent()
    {

    }

    //클릭 했을때 오브젝트 태그 확인
    private GameObject OnClickObjTag(string tag)
    {
        Vector3 touchPos = Vector3.zero;

#if UNITY_EDITOR
        touchPos = Input.mousePosition;
#else
        touchPos = Input.GetTouch(0).position;
#endif

        Ray ray = Camera.main.ScreenPointToRay(touchPos);
        RaycastHit hitInfo;
        Physics.Raycast(ray, out hitInfo);

        if (hitInfo.collider != null)
        {
            GameObject hitObject = hitInfo.collider.gameObject;
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
