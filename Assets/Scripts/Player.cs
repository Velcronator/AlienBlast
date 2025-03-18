using System;
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


    public bool IsGrounded;
    public bool IsOnSnow;

    AudioSource _audioSource;
    Animator _animator;
    SpriteRenderer _spriteRenderer;
    Collider2D _collider;
    Rigidbody2D _rb;
    PlayerInput _playerInput;
    float _jumpEndTime;
    float _horizontal;
    int _jumpsRemaining;

    PlayerData _playerData = new PlayerData();

    public event Action CoinsChanged;
    public event Action HealthChanged;

    public int Coins { get => _playerData.Coins; private set => _playerData.Coins = value; }
    public int Health { get => _playerData.Health; }

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _audioSource = GetComponent<AudioSource>();
        _rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        // todo : this is not performant way to do this


        FindFirstObjectByType<PlayerCanvas>().Bind(this);
    }

    void OnDrawGizmos()
    {
        if (_collider == null)
            _collider = GetComponent<Collider2D>();

        Gizmos.color = Color.red;

        Vector2 origin = new Vector2(transform.position.x, transform.position.y - _collider.bounds.extents.y);
        Gizmos.DrawLine(origin, origin + Vector2.down * 0.1f);

        // Draw Left Foot
        origin = new Vector2(transform.position.x - _footOffset, transform.position.y - _collider.bounds.extents.y);
        Gizmos.DrawLine(origin, origin + Vector2.down * 0.1f);

        // Draw Right Foot
        origin = new Vector2(transform.position.x + _footOffset, transform.position.y - _collider.bounds.extents.y);
        Gizmos.DrawLine(origin, origin + Vector2.down * 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGrounding();

        var _horizontalInput = _playerInput.actions["Move"].ReadValue<Vector2>().x;
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

        var vesiredHorizontal = _horizontalInput * _maxHorizontalSpeed;
        var acceleration = IsOnSnow ? _snowAcceleration : _groundAcceleration;

        _horizontal = Mathf.Lerp(_horizontal, vesiredHorizontal, Time.deltaTime * acceleration);
        _rb.linearVelocity = new Vector2(_horizontal, vertical);
        UpdateSpriteAndAnimation();
    }

    void UpdateGrounding()
    {
        IsGrounded = false;
        IsOnSnow = false;

        // Check Center
        Vector2 origin = new Vector2(transform.position.x, transform.position.y - _collider.bounds.extents.y);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 0.1f, _layerMask);
        if (hit.collider)
        {
            IsGrounded = true;
            IsOnSnow = hit.collider.CompareTag("Snow");
        }

        // Check Left Foot
        origin = new Vector2(transform.position.x - _footOffset, transform.position.y - _collider.bounds.extents.y);
        hit = Physics2D.Raycast(origin, Vector2.down, 0.1f, _layerMask);
        if (hit.collider)
        {
            IsGrounded = true;
            IsOnSnow = hit.collider.CompareTag("Snow");
        }

        // Check Right Foot
        origin = new Vector2(transform.position.x + _footOffset, transform.position.y - _collider.bounds.extents.y);
        hit = Physics2D.Raycast(origin, Vector2.down, 0.1f, _layerMask);
        if (hit.collider)
        {
            IsGrounded = true;
            IsOnSnow = hit.collider.CompareTag("Snow");
        }

        if (IsGrounded && _rb.linearVelocity.y <= 0)
            _jumpsRemaining = _jumpsAllowed;
    }

    void UpdateSpriteAndAnimation()
    {
        _animator.SetBool("Jump", !IsGrounded);
        _animator.SetBool("Move", _horizontal != 0);

        _animator.SetFloat("HorizontalSpeed", Mathf.Abs(_horizontal));

        if (_horizontal > 0)
            _spriteRenderer.flipX = false;
        else if (_horizontal < 0)
            _spriteRenderer.flipX = true;
    }

    public void CollectCoin()
    {
        Coins++;
        CoinsChanged?.Invoke();
        _audioSource.PlayOneShot(_coinSFX);
    }

    public void Bind(PlayerData playerData)
    {
        _playerData =  playerData;
    }
    
    public void TakeDamage(Vector2 hitNormal)   
    {
        _playerData.Health--;
        HealthChanged?.Invoke();
        _audioSource.PlayOneShot(_hurtSFX);
        if (_playerData.Health <= 0)
        {
            SceneManager.LoadScene(0);
            return;
        }
        _rb.AddForce(-hitNormal * _knockbackVelocity);
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