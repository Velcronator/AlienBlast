using UnityEngine;

public class CatBomb : MonoBehaviour
{
    [SerializeField] float _forceAmount = 300f;
    Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction)
    {
        transform.SetParent(null);
        _rb.AddForce(direction * _forceAmount);
    }
}
