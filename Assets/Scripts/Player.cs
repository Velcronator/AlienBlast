using UnityEngine;

public class Player : MonoBehaviour
{
    private float _jumpEndTime;
    [SerializeField] private float _jumpVelocity = 5;
    [SerializeField] private float _jumpDuration = 0.5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        Debug.Log(horizontal);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        var vertical = rb.linearVelocity.y;

        if (Input.GetButtonDown("Fire1"))
            _jumpEndTime = Time.time + _jumpDuration;

        if (Input.GetButton("Fire1") && _jumpEndTime > Time.time)
            vertical = _jumpVelocity;

        rb.linearVelocity = new Vector2(horizontal, vertical);
    }
}