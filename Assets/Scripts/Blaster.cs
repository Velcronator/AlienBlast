using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class Blaster : MonoBehaviour
{
    [SerializeField] BlasterShot _blasterShotPrefab;
    [SerializeField] Transform _blasterShotSpawnPoint;

    ObjectPool<BlasterShot> _pool;
    Player _player;
    PlayerInput _playerInput;

    void Awake()
    {
        _pool = new ObjectPool<BlasterShot>(AddNewBlasterShotToPool,
            t => t.gameObject.SetActive(true),
            t => t.gameObject.SetActive(false));
        _player = GetComponent<Player>();
        _playerInput = GetComponent<PlayerInput>();
        //_playerInput.actions["Attack"].performed += TryAttack;
    }

    BlasterShot AddNewBlasterShotToPool()
    {
        var shot = Instantiate(_blasterShotPrefab);
        shot.SetPool(_pool);
        return shot;
    }

    void TryAttack(InputAction.CallbackContext context)
    {
        BlasterShot shot = _pool.Get();// Instantiate(_blasterShotPrefab, _blasterShotSpawnPoint.position, Quaternion.identity);
        shot.Launch(_player.Direction, _blasterShotSpawnPoint.position);
    }

    private void Update()
    {
        if (_playerInput.actions["Attack"].ReadValue<float>() > 0)
        {
            BlasterShot shot = _pool.Get();
            shot.Launch(_player.Direction, _blasterShotSpawnPoint.position);
        }
    }
}
