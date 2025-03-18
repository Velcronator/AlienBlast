using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Blaster : MonoBehaviour
{
    [SerializeField] GameObject _blasterShotPrefab;
    PlayerInput _playerInput;

    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.actions["Attack"].performed += TryAttack;
    }

    void TryAttack(InputAction.CallbackContext context)
    {
        Instantiate(_blasterShotPrefab, transform.position, Quaternion.identity);
    }
}
