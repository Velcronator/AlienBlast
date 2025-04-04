using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] float _maxHorizontalSpeed = 5;
    [SerializeField] float _jumpVelocity = 5;
    [SerializeField] float _jumpDuration = 0.5f;
    [SerializeField] Sprite _jumpSprite;
    [SerializeField] LayerMask _layerMask;
    [SerializeField] float _footOffset = 0.5f;
    [SerializeField] int _jumpsAllowed = 2;
    [SerializeField] float _groundAcceleration = 10f;
    [SerializeField] float _snowAcceleration = 1f;
    [SerializeField] AudioClip _coinSFX;
    [SerializeField] AudioClip _hurtSFX;
    [SerializeField] float _knockbackVelocity = 200f;
    [SerializeField] Collider2D _duckingCollider;
    [SerializeField] Collider2D _standingCollider;

    public bool IsGrounded;
    public bool IsOnSnow;

    AudioSource _audioSource;
    Animator _animator;
    Rigidbody2D _rb;
    PlayerInput _playerInput;
    SpriteRenderer _spriteRenderer;
    float _horizontal;
    float _jumpEndTime;
    int _jumpsRemaining;

    PlayerData _playerData = new PlayerData();

    public event Action CoinsChanged;
    public event Action HealthChanged;

    public int Coins { get => _playerData.Coins; private set => _playerData.Coins = value; }
    public int Health { get => _playerData.Health; }
    public Vector2 Direction { get; private set; } = Vector2.right;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponentInChildren<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        // todo : this is not performant way to do this
        FindFirstObjectByType<PlayerCanvas>().Bind(this);
    }

    void OnEnable() => FindFirstObjectByType<CinemachineTargetGroup>()?.AddMember(transform, 1f, 1f);

    void OnDisable() => FindFirstObjectByType<CinemachineTargetGroup>()?.RemoveMember(transform);

    void OnDrawGizmos()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Gizmos.color = Color.red;

        Vector2 origin = new Vector2(transform.position.x, transform.position.y - spriteRenderer.bounds.extents.y);
        Gizmos.DrawLine(origin, origin + Vector2.down * 0.1f);

        // Draw Right Foot
        origin = new Vector2(transform.position.x - _footOffset, transform.position.y - spriteRenderer.bounds.extents.y);
        Gizmos.DrawLine(origin, origin + Vector2.down * 0.1f);

        // Draw Left foot
        origin = new Vector2(transform.position.x + _footOffset, transform.position.y - spriteRenderer.bounds.extents.y);
        Gizmos.DrawLine(origin, origin + Vector2.down * 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGrounding();

        if (GameManager.CinematicPlaying == false)
        {
            UpdateMovement();
        }
        UpdateAnimation();
        UpdateDirection();
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

            _audioSource.pitch = _jumpsRemaining > 0 ? 1f : 1.1f;

            _audioSource.Play();
        }

        if (_playerInput.actions["Jump"].ReadValue<float>() > 0 && _jumpEndTime > Time.time)
            vertical = _jumpVelocity;

        var desiredHorizontal = horizontalInput * _maxHorizontalSpeed;
        var acceleration = IsOnSnow ? _snowAcceleration : _groundAcceleration;

        _animator.SetBool("Duck", verticalInput < 0 && MathF.Abs(verticalInput) > MathF.Abs(horizontalInput));

        var isDucking = _animator.GetBool("IsDucking");

        if (isDucking)
            desiredHorizontal = 0;

        _duckingCollider.enabled = isDucking;
        _standingCollider.enabled = !isDucking;

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

        _rb.linearVelocity = new Vector2(_horizontal, vertical);
    }

    void UpdateGrounding()
    {
        IsGrounded = false;
        IsOnSnow = false;

        //Check center
        Vector2 origin = new Vector2(transform.position.x, transform.position.y - _spriteRenderer.bounds.extents.y);
        CheckGrounding(origin);

        //Check Left
        origin = new Vector2(transform.position.x - _footOffset, transform.position.y - _spriteRenderer.bounds.extents.y);
        CheckGrounding(origin);

        //Check Right
        origin = new Vector2(transform.position.x + _footOffset, transform.position.y - _spriteRenderer.bounds.extents.y);
        CheckGrounding(origin);

        if (IsGrounded && GetComponent<Rigidbody2D>().linearVelocity.y <= 0)
            _jumpsRemaining = _jumpsAllowed;
    }

    private void CheckGrounding(Vector2 origin)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, Vector2.down, 0.1f, _layerMask);

        foreach (var hit in hits)
        {
            if (hit.collider == null)
                continue;

            if (hit.collider.isTrigger &&
                hit.collider.GetComponent<Water>() == null)
                continue;

            IsGrounded = true;
            IsOnSnow |= hit.collider.CompareTag("Snow"); 
            //Debug.Log($"Touching {hit.collider}", hit.collider.gameObject);
        }
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
        _audioSource.PlayOneShot(_coinSFX);
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
        _audioSource.PlayOneShot(_hurtSFX);
        _rb.AddForce(-hitNormal * _knockbackVelocity);
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