using UnityEngine;

public class Blaster : Item
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

    public override void Use()
    {
        if (GameManager.CinematicPlaying == true) return;
        Attack();
    }
}
