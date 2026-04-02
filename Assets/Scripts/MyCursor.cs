using UnityEngine;
using UnityEngine.UI;

public class MyCursor : MonoBehaviour
{
    private RectTransform rectTransform;
    private Image cursorImage;

    public Sprite[] cursorSprites;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        cursorImage = GetComponent<Image>();

        if (cursorSprites.Length > 0)
        {
            cursorImage.sprite = cursorSprites[0];
        }

        Cursor.visible = false;
    }

    void Update()
    {
        rectTransform.position = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            if (cursorSprites.Length > 1) cursorImage.sprite = cursorSprites[1];
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (cursorSprites.Length > 0) cursorImage.sprite = cursorSprites[0];
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit HitInfo;

        if (Physics.Raycast(ray, out HitInfo))
        {
            if (HitInfo.collider.gameObject.name == "tray2")
            {
                if (cursorSprites.Length > 2) cursorImage.sprite = cursorSprites[2];
            }
            else if (HitInfo.collider.gameObject.name == "tray1")
            {
                if (cursorSprites.Length > 3) cursorImage.sprite = cursorSprites[3];
            }
            else
            {
                if (cursorSprites.Length > 0) cursorImage.sprite = cursorSprites[0];
            }
        }
        else
        {
            if (cursorSprites.Length > 0) cursorImage.sprite = cursorSprites[0];
        }
    }
}
