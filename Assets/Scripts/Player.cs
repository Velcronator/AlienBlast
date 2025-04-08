using Unity.Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    [SerializeField] float _maxHorizontalSpeed = 5;
    [SerializeField] float _jumpVelocity = 5;
    [SerializeField] float _jumpDuration = 0.5f;
    [SerializeField] Sprite _jumpSprite;
    [SerializeField] LayerMask _layerMask;
    [SerializeField] float _footOffset = 0.5f;
    [SerializeField] float _groundAcceleration = 10;
    [SerializeField] float _snowAcceleration = 1;
    [SerializeField] AudioClip _coinSfx;
    [SerializeField] AudioClip _hurtSfx;
    [SerializeField] float _knockbackVelocity = 400;
    [SerializeField] Collider2D _duckingCollider;
    [SerializeField] Collider2D _standingCollider;
    [SerializeField] float _wallDetectionDistance = 0.5f;
    [SerializeField] int _wallCheckPoints = 5;
    [SerializeField] float _buffer = 0.1f;

    public bool IsGrounded;
    public bool IsOnSnow;
    public bool IsDucking;
    public bool IsTouchingRightWall;
    public bool IsTouchingLeftWall;

    Animator _animator;
    AudioSource _audioSource;
    Rigidbody2D _rb;
    PlayerInput _playerInput;
    Collider2D _mainCollider;

    float _horizontal;
    int _jumpsRemaining;
    float _jumpEndTime;

    PlayerData _playerData = new PlayerData();
    RaycastHit2D[] _results = new RaycastHit2D[100];

    public event Action CoinsChanged;
    public event Action HealthChanged;

    public int Coins { get => _playerData.Coins; private set => _playerData.Coins = value; }
    public int Health => _playerData.Health;

    public Vector2 Direction { get; private set; } = Vector2.right;

    Collider2D ActiveCollider => IsDucking ? _duckingCollider : _standingCollider;

    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _mainCollider = _standingCollider;
        FindFirstObjectByType<PlayerCanvas>().Bind(this);
    }

    void OnEnable() => FindFirstObjectByType<CinemachineTargetGroup>()?.AddMember(transform, 1f, 1f);
    void OnDisable() => FindFirstObjectByType<CinemachineTargetGroup>()?.RemoveMember(transform);

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        foreach (var point in GetGroundCheckPoints())
            Gizmos.DrawLine(point, point + Vector2.down * 0.1f);

        DrawGizmosForSide(Vector2.left);
        DrawGizmosForSide(Vector2.right);
    }

    private Vector2[] GetGroundCheckPoints()
    {
        var bounds = ActiveCollider.bounds;
        Vector2 basePos = new Vector2(bounds.center.x, bounds.min.y);
        return new Vector2[]
        {
            basePos,
            new Vector2(basePos.x - _footOffset, basePos.y),
            new Vector2(basePos.x + _footOffset, basePos.y)
        };
    }

    private void DrawGizmosForSide(Vector2 direction)
    {
        var bounds = ActiveCollider.bounds;
        float height = bounds.size.y - 2 * _buffer;
        float segmentSize = height / (_wallCheckPoints - 1);

        for (int i = 0; i < _wallCheckPoints; i++)
        {
            Vector2 baseOrigin = new Vector2(bounds.center.x, bounds.min.y + _buffer + segmentSize * i);
            Vector2 offsetOrigin = baseOrigin + direction * _wallDetectionDistance;
            Gizmos.DrawWireSphere(offsetOrigin, 0.05f);
        }
    }

    private bool CheckForWall(Vector2 direction)
    {
        var bounds = ActiveCollider.bounds;
        float height = bounds.size.y - 2 * _buffer;
        float segmentSize = height / (_wallCheckPoints - 1);

        for (int i = 0; i < _wallCheckPoints; i++)
        {
            Vector2 origin = new Vector2(bounds.center.x, bounds.min.y + _buffer + segmentSize * i);
            origin += direction * _wallDetectionDistance;

            int hits = Physics2D.Raycast(origin,
                direction,
                new ContactFilter2D() { layerMask = _layerMask, useLayerMask = true, useTriggers = true },
                _results,
                0.1f);

            for (int hitIndex = 0; hitIndex < hits; hitIndex++)
            {
                var hit = _results[hitIndex];
                if (hit.collider != null && !hit.collider.isTrigger)
                    return true;
            }
        }

        return false;
    }

    void Update()
    {
        UpdateGrounding();
        UpdateWallTouching();

        if (!GameManager.CinematicPlaying)
        {
            UpdateMovement();
        }

        UpdateAnimation();
        UpdateDirection();
    }

    private void UpdateWallTouching()
    {
        IsTouchingRightWall = CheckForWall(Vector2.right);
        IsTouchingLeftWall = CheckForWall(Vector2.left);
    }

    private void UpdateMovement()
    {
        var input = _playerInput.actions["Move"].ReadValue<Vector2>();
        var horizontalInput = input.x;
        var verticalInput = input.y;

        var vertical = _rb.linearVelocity.y;

        if (_playerInput.actions["Jump"].WasPerformedThisFrame() && _jumpsRemaining > 0)
        {
            _jumpEndTime = Time.time + _jumpDuration;
            _jumpsRemaining--;

            _audioSource.pitch = _jumpsRemaining > 0 ? 1 : 1.2f;
            _audioSource.Play();
        }

        if (_playerInput.actions["Jump"].ReadValue<float>() > 0 && _jumpEndTime > Time.time)
            vertical = _jumpVelocity;

        var desiredHorizontal = horizontalInput * _maxHorizontalSpeed;
        var acceleration = IsOnSnow ? _snowAcceleration : _groundAcceleration;

        _animator.SetBool("Duck", verticalInput < -0 && Mathf.Abs(verticalInput) > Mathf.Abs(horizontalInput));

        IsDucking = _animator.GetBool("IsDucking");
        if (IsDucking)
            desiredHorizontal = 0;

        _duckingCollider.enabled = IsDucking;
        _standingCollider.enabled = !IsDucking;

        if (desiredHorizontal > _horizontal)
        {
            _horizontal += acceleration * Time.deltaTime;
            if (_horizontal > desiredHorizontal)
                _horizontal = desiredHorizontal;
        }
        else if (desiredHorizontal < _horizontal)
        {
            _horizontal -= acceleration * Time.deltaTime;
            if (_horizontal < desiredHorizontal)
                _horizontal = desiredHorizontal;
        }

        if (desiredHorizontal > 0 && IsTouchingRightWall)
            _horizontal = 0;
        if (desiredHorizontal < 0 && IsTouchingLeftWall)
            _horizontal = 0;

        _rb.linearVelocity = new Vector2(_horizontal, vertical);
    }

    void UpdateGrounding()
    {
        IsGrounded = false;
        IsOnSnow = false;

        foreach (var point in GetGroundCheckPoints())
        {
            CheckGrounding(point);
        }

        if (IsGrounded && _rb.linearVelocity.y <= 0)
            _jumpsRemaining = 2;
    }

    private void CheckGrounding(Vector2 origin)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 0.1f, _layerMask);

        if (!hit.collider)
        {
            return;
        }

        if (hit.collider.isTrigger && !hit.collider.TryGetComponent(out Water _))
        {
            return;
        }

        IsGrounded = true;
        IsOnSnow = hit.collider.CompareTag("Snow");
    }

    void UpdateAnimation()
    {
        _animator.SetBool("Jump", !IsGrounded);
        _animator.SetBool("Move", _horizontal != 0);
    }

    void UpdateDirection()
    {
        if (_horizontal > 0)
        {
            _animator.transform.rotation = Quaternion.identity;
            Direction = Vector2.right;
        }
        else if (_horizontal < 0)
        {
            _animator.transform.rotation = Quaternion.Euler(0, 180, 0);
            Direction = Vector2.left;
        }
    }

    public void CollectCoin()
    {
        Coins++;
        _audioSource.PlayOneShot(_coinSfx);
        CoinsChanged?.Invoke();
    }

    public void Bind(PlayerData playerData)
    {
        _playerData = playerData;
    }

    public void TakeDamage(Vector2 hitNormal)
    {
        _playerData.Health--;
        if (_playerData.Health <= 0)
        {
            SceneManager.LoadScene(0);
            return;
        }
        _rb.AddForce(-hitNormal * _knockbackVelocity);
        _audioSource.PlayOneShot(_hurtSfx);
        HealthChanged?.Invoke();
    }

    public void StopJump()
    {
        _jumpEndTime = Time.time;
    }

    public void Bounce(Vector2 normal, float bounciness)
    {
        _rb.AddForce(-normal * bounciness);
    }
}
