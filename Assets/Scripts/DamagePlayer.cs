using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    [SerializeField] bool _ignoreFromTop;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (_ignoreFromTop)
        {
            Vector2 normal = collision.contacts[0].normal;
            float dot = Vector2.Dot(normal, Vector2.down);
            if (dot > 0.5)
                return;
        }

        if (collision.collider.CompareTag("Player"))
        {
            var player = collision.collider.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(collision.contacts[0].normal);
            }
        }
    }
}
