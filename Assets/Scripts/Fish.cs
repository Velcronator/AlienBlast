using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class Fish : MonoBehaviour
{
    [SerializeField] SplineAnimate _splineAnimate;
    [SerializeField] Animator _animator;
    [SerializeField] SplineAttackPoints _splineAttackPoints;

    float _nextAttackPoint;
    Queue<float> _attackPoints;

    void Start()
    {
        GetComponentInChildren<ShootAnimationWrapper>().OnShoot += ShootSpikes;
        RefreshAttackPoints();
    }

    private void ShootSpikes()
    {
        Debug.Log("Shoot Spikes");
    }

    void RefreshAttackPoints()
    {
        _attackPoints = _splineAttackPoints.GetAsQueue();
        _nextAttackPoint = _attackPoints.Dequeue();
    }

    void Update()
    {
        var elapsedTime = _splineAnimate.NormalizedTime % 1f;
        if(elapsedTime >= _nextAttackPoint)
        {
            Attack();
            if (_attackPoints.Any())
            {
                _nextAttackPoint = _attackPoints.Dequeue();
            }
            else
            {
                RefreshAttackPoints();
            }
        }
    }

    private void Attack()
    {
        _animator.SetTrigger("Attack");
    }
}
