using UnityEngine;

public class Cat : MonoBehaviour
{
    [SerializeField] GameObject _catBombPrefab;
    [SerializeField] Transform _firePoint;

    void Start()
    {
        InvokeRepeating(nameof(SpawnCatBomb), 3f, 3f);
    }

    private void SpawnCatBomb()
    {
        Instantiate(_catBombPrefab, _firePoint);
    }
}
