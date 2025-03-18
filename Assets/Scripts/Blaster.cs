using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Blaster : MonoBehaviour
{
    [SerializeField] BlasterShot _blasterShotPrefab;
    [SerializeField] Transform _blasterShotSpawnPoint;

    Player _player;
    PlayerInput _playerInput;

    void Awake()
    {
        _player = GetComponent<Player>();
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.actions["Attack"].performed += TryAttack;
    }

    void TryAttack(InputAction.CallbackContext context)
    {
        BlasterShot shot = Instantiate(_blasterShotPrefab, _blasterShotSpawnPoint.position, Quaternion.identity);
        shot.Launch(_player.Direction);
    }
}
