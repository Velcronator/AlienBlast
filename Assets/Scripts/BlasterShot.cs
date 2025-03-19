using UnityEngine;

public class BlasterShot : MonoBehaviour
{
    [SerializeField] float _speed = 5f;
    [SerializeField] GameObject _impactExplosion;

    Rigidbody2D _rb;
    Vector2 _direction = Vector2.right;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        _rb.linearVelocity = _direction * _speed;
    }

    public void Launch(Vector2 direction)
    {
        _direction = direction;
        transform.rotation = _direction == Vector2.right ? Quaternion.identity : Quaternion.Euler(0, 180, 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var damagable = collision.gameObject.GetComponent<ITakeDamage>();
        if (damagable != null)
            damagable.TakeDamage();

        var explosion = Instantiate(_impactExplosion, collision.contacts[0].point, Quaternion.identity);
        Destroy(explosion.gameObject, 0.9f);

        gameObject.SetActive(false);
    }
}
