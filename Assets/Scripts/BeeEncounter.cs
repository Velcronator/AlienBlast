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
    [SerializeField] Transform[] _beeDestinations;

    List<Transform> _activeLightnings;
    public int _health = 5;
    public int _destinationIndex;


    private void OnValidate()
    {
        if (_lightningAnimationTime <= _delayBeforeDamage)
            _delayBeforeDamage = _lightningAnimationTime;
        // make sure the input is valid
        _numberOfLightnings = Mathf.Clamp(_numberOfLightnings, 0, _lightnings.Count);
    }

    void OnEnable()
    {
        StartCoroutine(StartLightning());
        StartCoroutine(StartMovement());
    }

    IEnumerator StartMovement()
    {
        GrabBag<Transform> grabBag = new GrabBag<Transform>(_beeDestinations);

        while (true)
        {
            var destination = grabBag.Grab();
            if (destination == null)
            {
                Debug.LogError("Unable to choose a random destination for the Bee. Stopping Movement");
                yield break;
            }

            while (Vector2.Distance(_bee.transform.position, destination.position) > 0.1f)
            {
                _bee.transform.position = Vector2.MoveTowards(_bee.transform.position, destination.position, Time.deltaTime);
                yield return null;
            }
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
        _health--;
        if (_health <= 0)
        {
            _bee.SetActive(false);
        }
    }
}
