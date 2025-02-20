using System.Collections;
using UnityEngine;

public class Spring : MonoBehaviour
{
    [SerializeField] Sprite _sprung;
    [SerializeField] float _springDelay = 0.2f;

    AudioSource _audioSource;
    SpriteRenderer _spriteRenderer;
    Sprite _default;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _default = GetComponent<SpriteRenderer>().sprite;
        _audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            _spriteRenderer.sprite = _sprung;
            _audioSource.Play();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            StartCoroutine(ResetSpriteAfterDelay(_springDelay));
        }
    }

    private IEnumerator ResetSpriteAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _spriteRenderer.sprite = _default;
    }
}
