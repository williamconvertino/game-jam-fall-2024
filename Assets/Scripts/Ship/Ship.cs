using UnityEngine;
public class Ship : Entity
{
    private ShipComponent[] _childrenComponents = new ShipComponent[] { };
    private float _mass = 0.0f;
    private void Start()
    {
        Initialize();
        if (!Frozen)
        {
            Unfreeze();
        }
    }

    public void Initialize()
    {
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
    public override void Freeze()
    {
        base.Freeze();
        foreach (var component in _childrenComponents)
        {
            component.Frozen = true;
        }
    }
    public override void Unfreeze()
    {
        base.Unfreeze();
        foreach (var component in _childrenComponents)
        {
            component.Frozen = false;
        }
    }

    public void Turn(float amount)
    {
        Rb2d.AddTorque(-amount);
    }

    public void Thrust(float amount, Vector3 direction, Vector3 position)
    {
        Rb2d.AddForceAtPosition( direction * amount, position);
    }

    private void CalculateMass()
    {
        _mass = 0.0f;
        foreach (var component in _childrenComponents)
        {
            _mass += component.Mass;
        }
        Rb2d.mass = _mass;
    }
}
