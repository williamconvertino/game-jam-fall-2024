using UnityEngine;

public class ShipComponent : MonoBehaviour
{
    public float Mass = 1.0f;
    public bool Frozen = true;
    [HideInInspector] public Ship ParentShip;

    public void Initialize(Ship parentShip)
    {
        ParentShip = parentShip;
    }
    
}