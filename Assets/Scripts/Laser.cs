using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] Vector2 _direction = Vector2.left;
    [SerializeField] float _distance = 10f;
    [SerializeField] SpriteRenderer _laserBurst;

    [SerializeField] LineRenderer _lineRenderer;
    bool _isOn;

    void Awake()
    {
        _lineRenderer.SetPosition(0, transform.position);
        Toggle(false);
    }

    public void Toggle(bool state)
    {
        _isOn = state;
        _lineRenderer.enabled = state;
    }

    void Update()
    {
        if (_isOn == false)
        {
            _laserBurst.enabled = false;
            return;
        }

        var endPoint = (Vector2)transform.position + (_direction * _distance);

        var firstThing = Physics2D.Raycast(transform.position, _direction, _distance);
        if (firstThing.collider)
        {
            endPoint = firstThing.point;

            _laserBurst.enabled = true;
            _laserBurst.transform.position = endPoint;
            _laserBurst.transform.localScale = Vector3.one * (0.5f + Mathf.PingPong(Time.time, 1f));

            var laserDamageable = firstThing.collider.GetComponent<ITakeLaserDamage>();
            if (laserDamageable != null)
                laserDamageable.TakeLaserDamage();
        }
        else
        {
            _laserBurst.enabled = false;
        }
        _lineRenderer.SetPosition(1, endPoint);
    }
}
