using UnityEngine;

public class Dog : MonoBehaviour, ITakeDamage
{
    void Start()
    {
        var shootAnimationWrapper = GetComponentInChildren<ShootAnimationWrapper>();
        shootAnimationWrapper.OnShoot += Shoot;
    }

    void Shoot()
    {
        Debug.Log("Dog shoots");
    }

    public void TakeDamage()
    {
        gameObject.SetActive(false);
    }
}
