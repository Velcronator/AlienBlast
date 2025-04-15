using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] GameObject _open1;
    [SerializeField] GameObject _open2;
    [SerializeField] GameObject _close1;
    [SerializeField] GameObject _close2;

    [ContextMenu(nameof(Open))]
    public void Open()
    {
        _open1.SetActive(true);
        _open2.SetActive(true);
        _close1.SetActive(false);
        _close2.SetActive(false);

        Debug.Log("Door opened!");
    }

    [ContextMenu(nameof(Close))]
    public void Close()
    {
        _open1.SetActive(false);
        _open2.SetActive(false);
        _close1.SetActive(true);
        _close2.SetActive(true);
        Debug.Log("Door closed!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var playerInteractionController = collision.GetComponent<PlayerInteractionController>();
        if (playerInteractionController)
        {
            playerInteractionController.Add(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var playerInteractionController = collision.GetComponent<PlayerInteractionController>();
        if (playerInteractionController)
        {
            playerInteractionController.Remove(this);
        }
    }

    public void Interact(PlayerInteractionController playerInteractionController)
    {
        var destination = Vector2.Distance(playerInteractionController.transform.position, _open1.transform.position) >
                        Vector2.Distance(playerInteractionController.transform.position, _open2.transform.position)
            ? _open1.transform.position
            : _open2.transform.position;
        playerInteractionController.transform.position = destination;
    }
}
