using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour
{
    //[SerializeField] private LayerMask _interactableLayerMask;
    //[SerializeField] private float _interactionDistance = 1f;
    //[SerializeField] private float _interactionCooldown = 0.5f;

    TMP_Text _interactionText;
    PlayerInput _playerInput;
    List<Door> _doors = new();

    private void Awake()
    {
        _interactionText = GetComponentInChildren<TMP_Text>();
        _interactionText.gameObject.SetActive(false);
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.actions["Interact"].performed += OnInteract;
    }

    private void OnEnable()
    {
        _playerInput.actions["Interact"].Enable();
    }

    private void OnDisable()
    {
        _playerInput.actions["Interact"].Disable();
    }

    private void OnDestroy()
    {
        _playerInput.actions["Interact"].performed -= OnInteract;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (_doors.Count > 0)
        {
            foreach (var door in _doors)
            {
                door.Interact(this);
            }
        }
    }

    public void Add(Door door)
    {
        _doors.Add(door);
        _interactionText.gameObject.SetActive(true);
    }
    public void Remove(Door door)
    {
        _doors.Remove(door);
        if (_doors.Count == 0)
        {
            _interactionText.gameObject.SetActive(false);
        }
    }
}
