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
    [SerializeField] int _spikeCount = 5;
    [SerializeField] int _spread = 15;
    [SerializeField] int _origin = 0;
    [SerializeField] float _fireSpeed =10f;

    float _nextAttackPoint;
    Queue<float> _attackPoints;

    void Start()
    {
        GetComponentInChildren<ShootAnimationWrapper>().OnShoot += ShootSpikes;
        RefreshAttackPoints();
    }

    private void ShootSpikes()
    {
        for (int i = 0; i < _spikeCount; i++)
        {
            float angle = i - (_spikeCount - 1) / 2f;
            float offset = _spread * angle;
            float finalAngle = _origin + offset;
            ReturnToPool spike = PoolManager.Instance.GetSpike();
            spike.transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0, 0, finalAngle));
            spike.GetComponent<Rigidbody2D>().linearVelocity = spike.transform.right * _fireSpeed;
        }
    }

    void RefreshAttackPoints()
    {
        _attackPoints = _splineAttackPoints.GetAsQueue();
        _nextAttackPoint = _attackPoints.Dequeue();
    }

    void Update()
    {
        var elapsedTime = _splineAnimate.NormalizedTime % 1f;
        if (elapsedTime >= _nextAttackPoint)
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
