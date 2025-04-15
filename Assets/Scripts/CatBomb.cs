using System;
using UnityEngine;
using UnityEngine.Pool;

public class CatBomb : MonoBehaviour
{
    [SerializeField] float _forceAmount = 300f;

    Rigidbody2D _rb;
    Animator _animator;
    private ObjectPool<CatBomb> _pool;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.simulated = false;
        _animator = GetComponentInChildren<Animator>();
        _animator.enabled = false;
    }

    public void Launch(Vector2 direction)
    {
        transform.SetParent(null);
        _rb.simulated = true;
        _rb.AddForce(direction * _forceAmount);
        _animator.enabled = true;
    }

    public void SetPool(ObjectPool<CatBomb> catBombPool)
    {
        _pool = catBombPool;
    }
}
