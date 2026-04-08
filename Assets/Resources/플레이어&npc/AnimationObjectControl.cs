using UnityEngine;

public class AnimationObjectControl : MonoBehaviour
{
    [Header("애니메이션 중 켜질 오브젝트")]
    public GameObject targetObject;

    // 1. 오브젝트를 켜는 함수 (애니메이션 시작 시점 등에 배치)
    public void ShowObject()
    {
        if (targetObject != null)
            targetObject.SetActive(true);
    }

    // 2. 오브젝트를 끄는 함수 (애니메이션 종료 시점 등에 배치)
    public void HideObject()
    {
        if (targetObject != null)
            targetObject.SetActive(false);
    }
}