using UnityEngine;

public class Key : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();
        if (player != null)
        {
            transform.SetParent(player.ItemPoint);
            transform.localPosition = Vector3.zero;
        }

    }
}
