using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    PlayerInput _playerInput;
    Key _equippedKey;
    public Transform ItemPoint;

    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.actions["Attack"].performed += UseEquippedItem;
    }

    void UseEquippedItem(InputAction.CallbackContext context)
    {
        if (_equippedKey)
            _equippedKey.Use();
    }

    public void Pickup(Key key)
    {
        key.transform.SetParent(ItemPoint);
        key.transform.localPosition = Vector3.zero;
        _equippedKey = key;
    }


}
