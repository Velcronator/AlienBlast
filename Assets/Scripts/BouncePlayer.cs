using UnityEngine;

public class BouncePlayer : MonoBehaviour
{
    [SerializeField] bool _onlyFromTop;
    [SerializeField] float _bounciness = 200f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (_onlyFromTop)
        {
            Vector2 normal = collision.contacts[0].normal;
            float dot = Vector2.Dot(normal, Vector2.down);
            if (dot < 0.5)
                return;
        }

        if (collision.collider.CompareTag("Player"))
        {
            var player = collision.collider.GetComponent<Player>();
            if (player != null)
            {
                player.Bounce(collision.contacts[0].normal, _bounciness);
            }
        }
    }
}
