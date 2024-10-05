using UnityEngine;

public class GunComponent : ShipComponent
{
    public KeyCode FireKey = KeyCode.LeftShift;
    private GunBarrel[] _barrels;

    private void Start()
    {
        _barrels = GetComponentsInChildren<GunBarrel>();
        foreach (var barrel in _barrels)
        {
            barrel.Initialize(this);
        }
    }
    
    private void Update()
    {
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
