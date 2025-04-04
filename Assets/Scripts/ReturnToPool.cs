using UnityEngine;
using UnityEngine.Pool;

public class ReturnToPool : MonoBehaviour
{
    [SerializeField] float _delay = 0.5f;
    private ObjectPool<ReturnToPool> _pool;
    
    void OnEnable()
    {
        Invoke(nameof(Release), _delay);
    }

    void Release() => _pool.Release(this);

    public void SetPool(ObjectPool<ReturnToPool> pool) => _pool = pool;
}