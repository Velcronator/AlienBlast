using UnityEngine;

public class Cat : MonoBehaviour
{
    [SerializeField] CatBomb _catBombPrefab;
    [SerializeField] Transform _firePoint;

    void Start()
    {
        InvokeRepeating(nameof(SpawnCatBomb), 3f, 3f);
    }

    private void SpawnCatBomb()
    {
        var catBomb = Instantiate(_catBombPrefab, _firePoint);
        catBomb.Launch(Vector2.up + Vector2.left);
    }
}
