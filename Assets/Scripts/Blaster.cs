using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class Blaster : MonoBehaviour
{
    [SerializeField] Transform _blasterShotSpawnPoint;

    Player _player;
    PlayerInput _playerInput;

    void Awake()
    {
        _player = GetComponent<Player>();
        _playerInput = GetComponent<PlayerInput>();
        //_playerInput.actions["Attack"].performed += TryAttack;
    }

    void TryAttack(InputAction.CallbackContext context)
    {
        BlasterShot shot = PoolManager.Instance.GetBlasterShot();
        shot.Launch(_player.Direction, _blasterShotSpawnPoint.position);
    }

    private void Update()
    {
        if (_playerInput.actions["Attack"].ReadValue<float>() > 0)
        {
            BlasterShot shot = PoolManager.Instance.GetBlasterShot();
            shot.Launch(_player.Direction, _blasterShotSpawnPoint.position);
        }
    }
}
