using UnityEngine;

public class Frog : MonoBehaviour
{
    [SerializeField] Sprite _jumpSprite;
    [SerializeField] Vector2 _jumpForce = new Vector2(3f, 3f);
    [SerializeField] float _minJumpInterval = 1f;
    [SerializeField] float _maxJumpInterval = 3f;

    Sprite _defaultSprite;
    SpriteRenderer _spriteRenderer;
    AudioSource _audioSource;
    Rigidbody2D _rb;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
        _defaultSprite = _spriteRenderer.sprite;

        Invoke("Jump", Random.Range(_minJumpInterval, _maxJumpInterval));
    }

    void Jump()
    {
        // play the jump sound
        _audioSource.Play();

        // Randomize the jump direction
        Vector2 jumpDirection = Random.value > 0.5f ? Vector2.right : Vector2.left;

        // flip the sprite if jumping right
        _spriteRenderer.flipX = jumpDirection == Vector2.right;
        jumpDirection += Vector2.up; // Add upward force

        // randomize the jump force x and y by 80 percent
        jumpDirection.x *= Random.Range(0.8f, 1.2f);
        jumpDirection.y *= Random.Range(0.8f, 1.2f);

        _spriteRenderer.sprite = _jumpSprite;
        _rb.AddForce(new Vector2(jumpDirection.x * _jumpForce.x, jumpDirection.y * _jumpForce.y), ForceMode2D.Impulse);

        // Schedule the next jump with a random interval
        Invoke("Jump", Random.Range(_minJumpInterval, _maxJumpInterval));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        _spriteRenderer.sprite = _defaultSprite;
    }
}
