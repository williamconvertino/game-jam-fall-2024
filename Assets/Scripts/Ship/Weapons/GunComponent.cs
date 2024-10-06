using UnityEngine;

public class GunComponent : ShipComponent
{
    public KeyCode FireKey = KeyCode.Z;
    private GunBarrel[] _barrels;

    public override void Start()
    {
        base.Start();
        connections[0] = false;
        connections[1] = false;
        connections[3] = false;
        
        _barrels = GetComponentsInChildren<GunBarrel>();
        foreach (var barrel in _barrels)
        {
            barrel.Initialize(this);
        }
    }

    protected override void Update()
    {
        base.Update();
        if (Frozen)
        {
            return;
        }
        if (Input.GetKeyDown(FireKey))
        {
            foreach (var barrel in _barrels)
            {
                barrel.TryFire();
            }
        }
    }
}
