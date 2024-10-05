using UnityEngine;

public class PilotComponent : ShipComponent
{
    [SerializeField] private float _forwardThrust = 0.0f;
    [SerializeField] private float _backwardThrust = 0.0f;
    
    [SerializeField] private float _turnSpeed = 1.0f;

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

        float turn = Input.GetAxis("Horizontal");
        if (turn != 0)
        {
            ParentShip.Turn(turn * _turnSpeed);
        }
    }
}
