using UnityEngine;

public class ThrusterComponent : ShipComponent
{
    [SerializeField] private float _forwardThrust = 4.0f;
    [SerializeField] private float _backwardThrust = 0.0f;
    private KeyCode _forwardKey = KeyCode.UpArrow;
    private KeyCode _backwardKey = KeyCode.DownArrow;
    public override void Initialize(Ship parentShip)
    {
        base.Initialize(parentShip);
        Vector3 rotation = transform.localRotation.eulerAngles;
        if (rotation.z == 0)
        {
            _forwardKey = KeyCode.DownArrow;
            _backwardKey = KeyCode.UpArrow;
        }
        else if (rotation.z == 180)
        {
            _forwardKey = KeyCode.UpArrow;
            _backwardKey = KeyCode.DownArrow;
        }
        else if (rotation.z == 90)
        {
            _forwardKey = KeyCode.RightArrow;
            _backwardKey = KeyCode.LeftArrow;
        }
        else if (rotation.z == 270)
        {
            _forwardKey = KeyCode.LeftArrow;
            _backwardKey = KeyCode.RightArrow;
        }
    }
    void Update()
    {
        if (Frozen)
        {
            return;
        }
        if (Input.GetKey(_forwardKey))
        {
            ParentShip.Thrust(_forwardThrust, -transform.up, transform.position);
        }
        if (Input.GetKey(_backwardKey))
        {
            ParentShip.Thrust(_backwardThrust, transform.up, transform.position);
        }
    }
}