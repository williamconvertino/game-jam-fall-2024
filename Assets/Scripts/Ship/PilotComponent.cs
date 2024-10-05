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
        if (Input.GetKey(KeyCode.UpArrow))
        {
            ParentShip.Thrust(_forwardThrust, transform.up, transform.position);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            ParentShip.Thrust(_backwardThrust, -transform.up, transform.position);
        }
        float turn = 0;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            turn += 1;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            turn -= 1;
        }
        if (turn != 0)
        {
            ParentShip.Turn(turn * _turnSpeed);
        }
    }
}
