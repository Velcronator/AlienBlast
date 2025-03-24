using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    [SerializeField] BlasterShot _blasterShotPrefab;

    ObjectPool<BlasterShot> _pool;

    public static PoolManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        _pool = new ObjectPool<BlasterShot>(AddNewBlasterShotToPool,
           t => t.gameObject.SetActive(true),
           t => t.gameObject.SetActive(false));
    }

    BlasterShot AddNewBlasterShotToPool()
    {
        var shot = Instantiate(_blasterShotPrefab);
        shot.SetPool(_pool);
        return shot;
    }

    public BlasterShot GetBlasterShot()
    {
        return _pool.Get();
    }
}