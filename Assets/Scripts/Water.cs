using UnityEngine;

public class Water : MonoBehaviour
{
    AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        _audioSource.Play();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        _audioSource.Stop();
    }

}
