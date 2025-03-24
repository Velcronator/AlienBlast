using System;
using UnityEngine;

public class Dog : MonoBehaviour, ITakeDamage
{
    public void Shoot()
    {
        Debug.Log("Dog shoots");
    }

    public void TakeDamage()
    {
        gameObject.SetActive(false);
    }
}
