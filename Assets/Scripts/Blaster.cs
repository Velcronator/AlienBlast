using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class Blaster : MonoBehaviour
{
    [SerializeField] Transform _blasterShotSpawnPoint;

    Animator _animator;
    Player _player;
    PlayerInput _playerInput;

    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _player = GetComponent<Player>();
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.actions["Attack"].performed += TryAttack;
    }

    void TryAttack(InputAction.CallbackContext context)
    {
        Attack();
    }

    private void Attack()
    {
        BlasterShot shot = PoolManager.Instance.GetBlasterShot();
        shot.Launch(_player.Direction, _blasterShotSpawnPoint.position);
        _animator.SetTrigger("Attack");
    }

    private void Update()
    {
        //rapid fire
        //if (_playerInput.actions["Attack"].ReadValue<float>() > 0) Attack();
    }
}
