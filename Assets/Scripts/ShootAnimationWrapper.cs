using System;
using UnityEngine;

public class ShootAnimationWrapper : MonoBehaviour
{
    public event Action OnShoot;
    public void Shoot() => OnShoot?.Invoke();
}
