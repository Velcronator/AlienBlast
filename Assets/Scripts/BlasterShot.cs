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
        var dog = collision.gameObject.GetComponent<Dog>();
        if (dog != null)
        {
            dog.TakeDamage();
        }
        var brick = collision.gameObject.GetComponent<Brick>();
        if (brick != null)
        {
            brick.TakeDamage();
        }
        var ladybug = collision.gameObject.GetComponent<Ladybug>();
        if (ladybug != null)
        {
            ladybug.TakeDamage();
        }
        gameObject.SetActive(false);
    }
}
