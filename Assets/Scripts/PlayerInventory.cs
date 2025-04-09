using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    public Transform ItemPoint;

    PlayerInput _playerInput;
    IItem EquippedItem => _items.Count >= _currentItemIndex ? _items[_currentItemIndex] : null;
    List<IItem> _items = new List<IItem>();
    int _currentItemIndex;

    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.actions["Attack"].performed += UseEquippedItem;
        _playerInput.actions["Next"].performed += EquipNext;
        _playerInput.actions["Previous"].performed += EquipPrevious;

        foreach (var item in GetComponentsInChildren<IItem>())
            Pickup(item);
    }

    private void OnDestroy()
    {
        _playerInput.actions["Attack"].performed -= UseEquippedItem;
        _playerInput.actions["Next"].performed -= EquipNext;
        _playerInput.actions["Previous"].performed -= EquipPrevious;
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
        if (EquippedItem != null)
            EquippedItem.Use();
    }

    public void Pickup(IItem item)
    {
        item.transform.SetParent(ItemPoint);
        item.transform.localPosition = Vector3.zero;
        _items.Add(item);
        _currentItemIndex = _items.Count - 1;
        ToggleEquippedItem();

        var collider = item.gameObject.GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;
    }
}
