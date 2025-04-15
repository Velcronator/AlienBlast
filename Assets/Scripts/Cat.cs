using UnityEngine;

public class Cat : MonoBehaviour
{
    [SerializeField] Transform _firePoint;

    CatBomb _catBomb;

    void Start()
    {
        SpawnCatBomb();
        var shootAnimationWrapper = GetComponentInChildren<ShootAnimationWrapper>();
        shootAnimationWrapper.OnShoot += ShootCatBomb;
        shootAnimationWrapper.OnReload += SpawnCatBomb;
    }

    void SpawnCatBomb()
    {
        if (_catBomb == null)
        {
            _catBomb = PoolManager.Instance.GetCatBomb();
            _catBomb.transform.SetParent(_firePoint);
            _catBomb.transform.localPosition = Vector3.zero;
        }
    }

    void ShootCatBomb()
    {
        _catBomb.Launch(Vector2.up + Vector2.left);
        _catBomb = null;
    }
}
