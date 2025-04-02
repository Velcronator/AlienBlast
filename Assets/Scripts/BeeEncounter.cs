using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeEncounter : MonoBehaviour
{
    [SerializeField] List<Transform> _lightnings;
    [SerializeField] float _delayBeforeDamage = 1.5f;
    [SerializeField] float _lightningAnimationTime = 2f;
    [SerializeField] float _delayBetweenLightning = 1f;
    [SerializeField] float _lightningRadius = 1f;
    [SerializeField] LayerMask _playerLayer;

    private void OnValidate()
    {
        if (_lightningAnimationTime <= _delayBeforeDamage)
            _delayBeforeDamage = _lightningAnimationTime;
    }

    void OnEnable()
    {
        StartCoroutine(StartEncounter());
    }

    IEnumerator StartEncounter()
    {
        while (true)
        {
            foreach (var lightning in _lightnings)
            {
                lightning.gameObject.SetActive(false);
            }
            yield return null;

            int index = UnityEngine.Random.Range(0, _lightnings.Count);
            _lightnings[index].gameObject.SetActive(true);
            yield return new WaitForSeconds(_delayBeforeDamage);
            DamagePlayersInRange(_lightnings[index]);
            yield return new WaitForSeconds(_lightningAnimationTime - _delayBeforeDamage);
            _lightnings[index].gameObject.SetActive(false);
            yield return new WaitForSeconds(_delayBetweenLightning);
        }
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