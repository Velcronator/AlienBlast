using UnityEngine;

public class Water : MonoBehaviour
{
    AudioSource _audioSource;

    private void Awake()
    {
        if (!TryGetComponent(out _audioSource))
        {
            Debug.LogWarning("No AudioSource component found on this GameObject.");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (_audioSource != null)
        {
            _audioSource.Play();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (_audioSource != null)
        {
            _audioSource.Stop();
        }
    }
}
