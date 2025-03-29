using UnityEngine;

public class Blaster : MonoBehaviour, IItem
{
    [SerializeField] Transform _blasterShotSpawnPoint;

    Player _player;

    void Awake()
    {
        _player = GetComponentInParent<Player>();
    }

    void Attack()
    {
        BlasterShot shot = PoolManager.Instance.GetBlasterShot();
        shot.Launch(_player.Direction, _blasterShotSpawnPoint.position);
    }

    public void Use()
    {
        Attack();
    }
}
