using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeEncounter : MonoBehaviour
{
    [SerializeField] List<Transform> _lightnings;
    //[SerializeField] float _lightningDamageDelay = 1.5f;
    //[SerializeField] float _lightningRadius = 2.5f;
    //[SerializeField] LayerMask _layerMask;
    //[SerializeField] float _lightningAnimationTime = 2.5f;
    //[SerializeField] float _shockTime = 0.5f;
    //[SerializeField] int _numOfSimultaneousLightnings = 2;
    [SerializeField] float _delayBetweenLightningStrikes = 2.5f;

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

            int index = Random.Range(0, _lightnings.Count);
            _lightnings[index].gameObject.SetActive(true);
            yield return new WaitForSeconds(_delayBetweenLightningStrikes);
        }
    }
}
