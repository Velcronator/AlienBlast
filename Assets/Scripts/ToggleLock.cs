using UnityEngine;
using UnityEngine.Events;

public class ToggleLock : MonoBehaviour
{
    [SerializeField] UnityEvent OnUnlock;
    bool _unlocked;
    SpriteRenderer _spriteRenderer;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _unlocked = false;
        _spriteRenderer.color = Color.gray;
    }

    [ContextMenu(nameof(Toggle))]
    public void Toggle()
    {
        _unlocked = !_unlocked;
        _spriteRenderer.color = _unlocked ? Color.white : Color.gray;
        if(_unlocked)
        {
            OnUnlock?.Invoke();
        }
    }
}