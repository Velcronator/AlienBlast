using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

public class CatBomb : MonoBehaviour
{
    [SerializeField] float _forceAmount = 300f;
    [SerializeField] float _maxLifetime = 4f;

    Rigidbody2D _rb;
    Animator _animator;
    ObjectPool<CatBomb> _pool;
    Coroutine _lifetimeCoroutine;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.simulated = false;
        _animator = GetComponentInChildren<Animator>();
        _animator.enabled = false;
    }

    IEnumerator AutoReleaseAfterTime()
    {
        yield return new WaitForSeconds(_maxLifetime);
        SelfDestruct();
    }

    public void Launch(Vector2 direction)
    {
        transform.SetParent(null);
        _rb.simulated = true;
        _rb.AddForce(direction * _forceAmount);
        _animator.enabled = true;

        // Start lifetime coroutine
        _lifetimeCoroutine = StartCoroutine(AutoReleaseAfterTime());
    }

    void SelfDestruct()
    {
        if (_lifetimeCoroutine != null)
        {
            StopCoroutine(_lifetimeCoroutine);
            _lifetimeCoroutine = null;
        }

        _rb.simulated = false;
        _rb.linearVelocity = Vector2.zero;
        _animator.enabled = false;

        if (_pool != null)
        {
            _pool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPool(ObjectPool<CatBomb> catBombPool)
    {
        _pool = catBombPool;
    }

    void OnDisable()
    {
        if (_lifetimeCoroutine != null)
        {
            StopCoroutine(_lifetimeCoroutine);
            _lifetimeCoroutine = null;
        }

        _rb.simulated = false;
        _rb.linearVelocity = Vector2.zero;
        _animator.enabled = false;
    }
}
