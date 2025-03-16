using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] Vector2 _direction = Vector2.left;
    [SerializeField] float _distance = 10f;

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
            return;

        var endPoint = (Vector2)transform.position + (_direction * _distance);

        var firstThing = Physics2D.Raycast(transform.position, _direction, _distance);
        if (firstThing.collider)
        {
            endPoint = firstThing.point;

            var laserDamageable = firstThing.collider.GetComponent<ITakeLaserDamage>();
            if (laserDamageable != null) // the null check is necessary for the case where the object hit by the laser is not ITakeLaserDamage
                laserDamageable.TakeLaserDamage();
        }
        _lineRenderer.SetPosition(1, endPoint);
    }
}