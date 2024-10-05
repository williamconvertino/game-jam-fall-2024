using UnityEngine;

public class ShipComponent : MonoBehaviour
{
    public float Mass = 1.0f;
    public bool Frozen = true;
    protected Ship ParentShip;

    public void Initialize(Ship parentShip)
    {
        this.ParentShip = parentShip;
    }
    
}