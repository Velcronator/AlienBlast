using UnityEngine;

public class Brick : MonoBehaviour
{
    [SerializeField] ParticleSystem _brickParticles;
    [SerializeField] float _hitAngle = 0.5f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        var player = collision.gameObject.GetComponent<Player>();
        if (player == null)
            return;

        Vector2 normal = collision.contacts[0].normal;
        float dot = Vector2.Dot(normal, Vector2.up);
        Debug.Log(dot);

        if (dot > _hitAngle)
        {
            player.StopJump();
            Instantiate(_brickParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}