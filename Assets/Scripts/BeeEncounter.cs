using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BeeEncounter : MonoBehaviour
{
    [SerializeField] List<Transform> _lightnings;
    [SerializeField] float _delayBeforeDamage = 1.5f;
    [SerializeField] float _lightningAnimationTime = 2f;
    [SerializeField] float _delayBetweenLightning = 1f;
    [SerializeField] float _delayBetweenStrikes = 0.25f;
    [SerializeField] float _lightningRadius = 1f;
    [SerializeField] LayerMask _playerLayer;
    [SerializeField] int _numberOfLightnings = 1;

    List<Transform> _activeLightnings;

    private void OnValidate()
    {
        if (_lightningAnimationTime <= _delayBeforeDamage)
            _delayBeforeDamage = _lightningAnimationTime;
        // make sure the input is valid
        _numberOfLightnings = Mathf.Clamp(_numberOfLightnings, 0, _lightnings.Count);
    }

    void OnEnable()
    {
        StartCoroutine(StartEncounter());
    }

    IEnumerator StartEncounter()
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
        //if (_activeLightnings.Count >= _lightnings.Count)
        //{
        //    Debug.LogError("The number of requested lightnings exceeds the total available lightnings.");
        //    yield break;
        //}

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
}
