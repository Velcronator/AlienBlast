using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    PlayerInput _playerInput;
    Key EquippedKey => _items.Count >= _currentItemIndex ? _items[_currentItemIndex] : null;
    public Transform ItemPoint;
    List<Key> _items = new List<Key>();
    int _currentItemIndex;

    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.actions["Attack"].performed += UseEquippedItem;
        _playerInput.actions["Next"].performed += EquipNext;
        _playerInput.actions["Previous"].performed += EquipPrevious;
    }

    private void EquipPrevious(InputAction.CallbackContext context)
    {
        _currentItemIndex--;
        if (_currentItemIndex < 0)
            _currentItemIndex = _items.Count - 1;
        ToggleEquippedItem();
    }

    void EquipNext(InputAction.CallbackContext context)
    {
        _currentItemIndex++;
        if (_currentItemIndex >= _items.Count)
            _currentItemIndex = 0;
        ToggleEquippedItem();
    }

    private void ToggleEquippedItem()
    {
        for (int i = 0; i < _items.Count; i++)
        {
            _items[i].gameObject.SetActive(i == _currentItemIndex);
        }
    }

    void UseEquippedItem(InputAction.CallbackContext context)
    {
        if (EquippedKey)
            EquippedKey.Use();
    }

    public void Pickup(Key key)
    {
        key.transform.SetParent(ItemPoint);
        key.transform.localPosition = Vector3.zero;
        _items.Add(key);
        _currentItemIndex = _items.Count - 1;
        ToggleEquippedItem();
    }
}
