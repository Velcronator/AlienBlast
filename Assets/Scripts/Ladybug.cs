using System;
using Unity.VisualScripting;
using UnityEngine;

public class Ladybug : MonoBehaviour, ITakeLaserDamage, ITakeDamage
{
    [SerializeField] float _speed = 1f;
    [SerializeField] float _raycastDistance = 0.2f;
    [SerializeField] LayerMask _forwardRaycastLayerMask;

    Vector2 _direction = Vector2.left;

    SpriteRenderer _spriteRenderer;
    Collider2D _collider;
    Rigidbody2D _rigidbody;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void OnDrawGizmos()
    {
        var collider = GetComponent<Collider2D>();

        Vector2 offset = _direction * collider.bounds.extents.x;
        Vector2 origin = (Vector2)transform.position + offset;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + (_direction * _raycastDistance));

        var downOrigin = GetDownRayPosition(collider);

        Gizmos.DrawLine(downOrigin, downOrigin + (Vector2.down * _raycastDistance));
    }

    Vector2 GetDownRayPosition(Collider2D collider)
    {
        var bounds = collider.bounds;
        if (_direction == Vector2.left)
        {
            return new Vector2(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y);
        }
        else
        {
            return new Vector2(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y);
        }
    }

    void Update()
    {
        CheckGroundInFront();
        CheckInFront();

        _rigidbody.linearVelocity = new Vector2(_direction.x * _speed, _rigidbody.linearVelocity.y);
    }

    private void CheckInFront()
    {
        Vector2 offset = _direction * _collider.bounds.extents.x;
        Vector2 origin = (Vector2)transform.position + offset;


        var hits = Physics2D.RaycastAll(origin, _direction, _raycastDistance, _forwardRaycastLayerMask);
        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject != gameObject)
            {
                _direction *= -1;
                _spriteRenderer.flipX = _direction == Vector2.right;
                break;
            }
        }
    }

    private void CheckGroundInFront()
    {
        bool canContinueWalking = false;
        var downOrigin = GetDownRayPosition(_collider);
        var downHits = Physics2D.RaycastAll(downOrigin, Vector2.down, _raycastDistance);
        foreach (var hit in downHits)
        {
            if (hit.collider != null && hit.collider.gameObject != gameObject)
                canContinueWalking = true;
        }

        if (canContinueWalking == false)
        {
            _direction *= -1;
            _spriteRenderer.flipX = _direction == Vector2.right;
            return;
        }
    }

    public void TakeLaserDamage()
    {
        _rigidbody.linearVelocity = Vector2.zero;
    }

    public void TakeDamage()
    {
        gameObject.SetActive(false);
    }
}