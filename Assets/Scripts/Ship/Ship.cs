using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class Ship : MonoBehaviour
{
    private Rigidbody2D _rb2d;
    private ShipComponent[] _childrenComponents = new ShipComponent[] { };
    private float _mass = 0.0f;
    
    public void Initialize()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        InitializeComponents();
        CalculateMass();
        Unfreeze();
    }

    private void InitializeComponents()
    {
        _childrenComponents = GetComponentsInChildren<ShipComponent>();
        foreach (var component in _childrenComponents)
        {
            component.Initialize(this);
        }
    }
    public void Freeze()
    {
        foreach (var component in _childrenComponents)
        {
            component.Frozen = true;
        }
    }
    public void Unfreeze()
    {
        foreach (var component in _childrenComponents)
        {
            component.Frozen = false;
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
