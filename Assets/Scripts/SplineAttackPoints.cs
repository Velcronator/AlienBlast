using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineAttackPoints : MonoBehaviour
{
    [SerializeField] SplineContainer _splineContainer;
    [SerializeField] List<float> _attackPoints;

    internal Queue<float> GetAsQueue()
    {
        return new Queue<float>(_attackPoints);
    }

    private void OnDrawGizmos()
    {
        foreach (var attackPoint in _attackPoints)
        {
            var position = _splineContainer.EvaluatePosition(attackPoint);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(position, 0.2f);
        }
    }
}



