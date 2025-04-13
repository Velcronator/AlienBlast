using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour, IBind<PlayerData>
{
    public Transform ItemPoint;

    PlayerInput _playerInput;
    Item EquippedItem => _items.Count >= _currentItemIndex ? _items[_currentItemIndex] : null;
    List<Item> _items = new List<Item>();
    int _currentItemIndex;
    PlayerData _data;

    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.actions["Attack"].performed += UseEquippedItem;
        _playerInput.actions["Next"].performed += EquipNext;
        _playerInput.actions["Previous"].performed += EquipPrevious;

        foreach (var item in GetComponentsInChildren<Item>())
            Pickup(item, false);
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

    public void Pickup(Item item, bool isNew = false)
    {
        item.transform.SetParent(ItemPoint);
        item.transform.localPosition = Vector3.zero;
        _items.Add(item);
        _currentItemIndex = _items.Count - 1;
        ToggleEquippedItem();

        var collider = item.gameObject.GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;

        if (isNew && _data.Items.Contains(item.name) == false)
            _data.Items.Add(item.name);
    }

    public void Bind(PlayerData data)
    {
        _data = data;
        foreach (var itemName in _data.Items)
        {
            var itemGameObject = GameObject.Find(itemName);
            if (itemGameObject != null && itemGameObject.TryGetComponent<Item>(out var item))
                Pickup(item);
            else
            {
                item = GameManager.Instance.GetItem(itemName);
                if (item != null)
                    Pickup(item);
            }
        }
    }
}
