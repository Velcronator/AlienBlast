using UnityEngine;
using UnityEngine.InputSystem;

public class Key : MonoBehaviour
{
    [SerializeField] float _useRange = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();
        if (player != null)
        {
            transform.SetParent(player.ItemPoint);
            transform.localPosition = Vector3.zero;
            var playerInput = player.GetComponent<PlayerInput>();
            playerInput.actions["Attack"].performed += UseKey;
        }
    }

    void UseKey(InputAction.CallbackContext obj)
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, _useRange);
        foreach (var hit in hits)
        {
            var toggleLock = hit.GetComponent<ToggleLock>();
            if (toggleLock)
            {
                toggleLock.Toggle();
                break;
            }
        }
    }
}
