using UnityEngine;

public class ThrusterComponent : ShipComponent
{
    [SerializeField] private float _forwardThrust = 4.0f;
    [SerializeField] private float _backwardThrust = 1.0f;

    void Update()
    {
        if (Frozen)
        {
            return;
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            ParentShip.Thrust(_forwardThrust, transform.up, transform.position);
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            ParentShip.Thrust(_backwardThrust, -transform.up, transform.position);
        }
    }
}