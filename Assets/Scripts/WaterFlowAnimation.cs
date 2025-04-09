using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class WaterFlowAnimation : MonoBehaviour
{
    [SerializeField] float _scrollSpeed = 1f;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        float x = Mathf.Repeat(Time.time * _scrollSpeed, 1);
        Vector2 offset = new Vector2(x, 0);
        _spriteRenderer.material.SetTextureOffset("_MainTex", offset);
    }
}
