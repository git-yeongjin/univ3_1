using UnityEngine;

public class TextureScroll : MonoBehaviour
{
    public float scrollSpeed = 0.5f;
    // 셰이더 그래프에서 설정한 변수의 'Reference' 이름을 넣으세요.
    public string texturePropertyName = "_MainTex";

    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        float yOffset = Time.time * scrollSpeed;

        // mainTextureOffset 대신 SetTextureOffset을 사용해 이름을 직접 지정합니다.
        rend.material.SetTextureOffset(texturePropertyName, new Vector2(0, yOffset));
    }
}