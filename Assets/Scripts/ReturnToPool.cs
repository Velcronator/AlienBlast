using UnityEngine;
using UnityEngine.Pool;

public class ReturnToPool : MonoBehaviour
{
    [SerializeField] float _delay = 0.5f;
    private ObjectPool<ReturnToPool> _pool;
    
    void OnEnable()
    {
        CancelInvoke(); // Prevent duplicate invokes if object reactivates quickly
        Invoke(nameof(Release), _delay);
    }
    void OnDisable()
    {
        CancelInvoke(); // Clean up any pending invokes when disabled
    }

    void Release() => _pool.Release(this);

    public void SetPool(ObjectPool<ReturnToPool> pool) => _pool = pool;
}