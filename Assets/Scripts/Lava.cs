using UnityEngine;

public class Lava : MonoBehaviour
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
        if(collision.CompareTag("Player"))
        {
            // turn off the sprite renderer of the player
            collision.GetComponent<SpriteRenderer>().enabled = false;
        }
        if (_audioSource != null)
        {
            _audioSource.Play();
        }
    }

    //void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (_audioSource != null)
    //    {
    //        _audioSource.Stop();
    //    }
    //}
}
