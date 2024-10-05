using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class Ship : MonoBehaviour
{
    private Rigidbody2D _rb2d;
    private ShipComponent[] _childrenComponents;
    private float _mass = 0.0f;

    void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        InitializeComponents();
        CalculateMass();
    }

    private void InitializeComponents()
    {
        _childrenComponents = GetComponentsInChildren<ShipComponent>();
        foreach (var component in _childrenComponents)
        {
            component.Initialize(this);
        }
    }

    public void Turn(float amount)
    {
        _rb2d.AddTorque(-amount);
    }

    public void Thrust(float amount, Vector3 direction, Vector3 position)
    {
        _rb2d.AddForceAtPosition( direction * amount, position);
    }

    private void CalculateMass()
    {
        _mass = 0.0f;
        foreach (var component in _childrenComponents)
        {
            _mass += component.Mass;
        }
        _rb2d.mass = _mass;
    }
}
