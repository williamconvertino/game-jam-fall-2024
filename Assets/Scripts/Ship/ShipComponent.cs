using UnityEngine;

public class ShipComponent : MonoBehaviour
{
    public float Mass = 1.0f;
    protected Ship ParentShip;

    public void Initialize(Ship parentShip)
    {
        this.ParentShip = parentShip;
    }
    
}