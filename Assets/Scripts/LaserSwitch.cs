using UnityEngine;
using UnityEngine.Events;

public class LaserSwitch : MonoBehaviour
{
    [SerializeField] Sprite _left;
    [SerializeField] Sprite _right;

    [SerializeField] UnityEvent _on;
    [SerializeField] UnityEvent _off;

    SpriteRenderer _spriteRenderer;
    
    LaserSwitchData _data;

    public void Bind(LaserSwitchData data)
    {
        _data = data;
        UpdateSwitchState();
    }

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();
        if (player == null)
            return;

        var rigidbody = player.GetComponent<Rigidbody2D>();
        if (rigidbody.linearVelocity.x > 0)
            TurnOn();
        else if (rigidbody.linearVelocity.x < 0)
            TurnOff();
    }

    void TurnOff()
    {
        if (_data.IsOn)
        {
            _data.IsOn = false;
            UpdateSwitchState();
        }
    }

    void TurnOn()
    {
        if (!_data.IsOn)
        {
            _data.IsOn = true;
            UpdateSwitchState();
        }
    }

    void UpdateSwitchState()
    {
        if (_data.IsOn)
        {
            _on.Invoke();
            _spriteRenderer.sprite = _right;
        }
        else
        {
            _off.Invoke();
            _spriteRenderer.sprite = _left;
        }
    }
}