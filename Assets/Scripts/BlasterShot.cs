using UnityEngine;

public class BlasterShot : MonoBehaviour
{
    [SerializeField] float _speed = 5f;

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
        {
            damagable.TakeDamage();
        }
        gameObject.SetActive(false);
    }
}
