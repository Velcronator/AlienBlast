using System;
using UnityEngine;

public class Dog : MonoBehaviour
{
    public void Shoot()
    {
        Debug.Log("Dog shoots");
    }

    public void TakeDamage()
    {
        Debug.Log("Dog takes damage");
        gameObject.SetActive(false);
    }
}
