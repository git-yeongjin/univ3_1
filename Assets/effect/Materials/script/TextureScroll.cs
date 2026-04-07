using UnityEngine;

public class TextureScroll : MonoBehaviour
{
    public float scrollSpeed = 0.5f;
    Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        // 시간에 따라 텍스처의 좌표(Offset)를 계속 이동시킵니다.
        float offset = Time.time * scrollSpeed;
        rend.material.SetTextureOffset("_BaseMap", new Vector2(0, offset));
    }
}