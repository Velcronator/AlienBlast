using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BeeEncounter : MonoBehaviour, ITakeDamage
{
    [SerializeField] List<Transform> _lightnings;
    [SerializeField] float _delayBeforeDamage = 1.5f;
    [SerializeField] float _lightningAnimationTime = 2f;
    [SerializeField] float _delayBetweenLightning = 1f;
    [SerializeField] float _delayBetweenStrikes = 0.25f;
    [SerializeField] float _lightningRadius = 1f;
    [SerializeField] LayerMask _playerLayer;
    [SerializeField] int _numberOfLightnings = 1;
    [SerializeField] GameObject _bee;
    [SerializeField] Animator _beeAnimator;
    [SerializeField] Rigidbody2D _beeRigidbody2D;
    [SerializeField] Transform[] _beeDestinations;
    [SerializeField] float _minIdleTime = 1f;
    [SerializeField] float _maxIdleTime = 2f;
    [SerializeField] GameObject _beeLaser;
    [SerializeField] int _maxHealth = 50;
    [SerializeField] Water _water;
    [SerializeField] Collider2D[] _floodGroundColliders;

    List<Transform> _activeLightnings;
    int _currentHealth = 50;
    bool _shootStarted;
    bool _shootFinished;

    private void OnValidate()
    {
        if (_lightningAnimationTime <= _delayBeforeDamage)
            _delayBeforeDamage = _lightningAnimationTime;
        // make sure the input is valid
        _numberOfLightnings = Mathf.Clamp(_numberOfLightnings, 0, _lightnings.Count);
    }

    void OnEnable()
    {
        _currentHealth = _maxHealth;
        StartCoroutine(StartLightning());
        StartCoroutine(StartMovement());
        var wrapper = GetComponentInChildren<ShootAnimationWrapper>();
        wrapper.OnShoot += () => _shootStarted = true;
        wrapper.OnReload += () => _shootFinished = true;
    }

    IEnumerator StartMovement()
    {
        _beeLaser.SetActive(false);
        GrabBag<Transform> grabBag = new GrabBag<Transform>(_beeDestinations);

        while (true)
        {
            var destination = grabBag.Grab();
            if (destination == null)
            {
                Debug.LogError("Unable to choose a random destination for the Bee. Stopping Movement");
                yield break;
            }

            _beeAnimator.SetBool("Move", true);

            while (Vector2.Distance(_bee.transform.position, destination.position) > 0.1f)
            {
                _bee.transform.position = Vector2.MoveTowards(_bee.transform.position, destination.position, Time.deltaTime);
                yield return null;
            }

            _beeAnimator.SetBool("Move", false);
            yield return new WaitForSeconds(Random.Range(_minIdleTime, _maxIdleTime));
            _beeAnimator.SetTrigger("Attack");

            yield return new WaitUntil(() => _shootStarted);
            _shootStarted = false;
            _beeLaser.SetActive(true);

            yield return new WaitUntil(() => _shootFinished);
            _shootFinished = false;
            _beeLaser.SetActive(false);
        }
    }

    IEnumerator StartLightning()
    {
        foreach (var lightning in _lightnings)
        {
            lightning.gameObject.SetActive(false);
        }

        _activeLightnings = new List<Transform>();

        while (true)
        {
            for (int i = 0; i < _numberOfLightnings; i++)
            {
                yield return SpawnNewLightning();
            }

            yield return new WaitUntil(() => _activeLightnings.All(t => !t.gameObject.activeSelf));
            _activeLightnings.Clear();
        }
    }
    private IEnumerator SpawnNewLightning()
    {
        int index = Random.Range(0, _lightnings.Count);
        var lightning = _lightnings[index];

        while (_activeLightnings.Contains(lightning))
        {
            index = Random.Range(0, _lightnings.Count);
            lightning = _lightnings[index];
        }

        StartCoroutine(ShowLightning(lightning));
        _activeLightnings.Add(lightning);

        yield return new WaitForSeconds(_delayBetweenStrikes);
    }

    IEnumerator ShowLightning(Transform selectedLightning)
    {
        selectedLightning.gameObject.SetActive(true);
        yield return new WaitForSeconds(_delayBeforeDamage);
        DamagePlayersInRange(selectedLightning);
        yield return new WaitForSeconds(_lightningAnimationTime - _delayBeforeDamage);
        selectedLightning.gameObject.SetActive(false);
        yield return new WaitForSeconds(_delayBetweenLightning);
    }

    void DamagePlayersInRange(Transform lightning)
    {
        Collider2D[] hitResults = Physics2D.OverlapCircleAll(lightning.position, _lightningRadius, _playerLayer);

        foreach (Collider2D hit in hitResults)
        {
            Player player = hit.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(lightning.position);
                //player.TakeDamage(Vector3.zero); if you want the player to not be pushed back
            }
        }
    }

    public void TakeDamage()
    {
        _currentHealth--;
        if (_currentHealth == _maxHealth / 2)
        {
            StartCoroutine(ToggleFlood(true));
        }

        if (_currentHealth <= 0)
        {
            StopAllCoroutines();
            StartCoroutine(ToggleFlood(false));
            _beeAnimator.SetBool("Dead", true);
            _beeRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            foreach (var collider in _bee.GetComponentsInChildren<Collider2D>())
            {
                collider.gameObject.layer = LayerMask.NameToLayer("Dead");
            }
        }
        else
        {
            _beeAnimator.SetTrigger("Hit");
        }
    }

    // Update the ToggleFlood method
    IEnumerator ToggleFlood(bool enableFlood)
    {
        float initialWaterY = _water.transform.position.y;
        float targetWaterY = enableFlood ? initialWaterY + 1 : initialWaterY - 1;
        float duration = 1f;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            float y = Mathf.Lerp(initialWaterY, targetWaterY, progress);
            var destination = new Vector3(_water.transform.position.x, y, _water.transform.position.z);
            _water.transform.position = destination;
            yield return null;
        }
        foreach (var collider in _floodGroundColliders)
        {
            collider.enabled = !enableFlood;
        }
        _water.SetSpeed(enableFlood ? 5f : 0);
    }

    // Debugging stuff
    [ContextMenu(nameof(HalfHealth))]
    void HalfHealth()
    {
        _currentHealth = _maxHealth / 2;
        _currentHealth++;
        TakeDamage();
    }

    [ContextMenu(nameof(FullHealth))]
    void FullHealth()
    {
        _currentHealth = _maxHealth;
    }

    [ContextMenu(nameof(Kill))]
    void Kill()
    {
        _currentHealth = 1;
        TakeDamage();
    }

}
