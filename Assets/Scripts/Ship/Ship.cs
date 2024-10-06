using UnityEngine;
public class Ship : Entity
{
    [HideInInspector] public int ShipSize = 0; 
    private ShipComponent[] _childrenComponents = new ShipComponent[] { };
    private ShipComponent[,] _componentGraph;
    private void Start()
    {
        Initialize(new ShipComponent[,]{});
        if (!Frozen)
        {
            Unfreeze();
        }
    }

    public void Initialize(ShipComponent[,] componentGraph)
    {
        _componentGraph = componentGraph;
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
        ShipSize = 0;
        foreach (var component in _childrenComponents)
        {
            if (component.CompareTag("Block"))
            {
                ShipSize++;
            }
        }

        Rb2d.mass = Mathf.Sqrt(ShipSize);
    }
}
