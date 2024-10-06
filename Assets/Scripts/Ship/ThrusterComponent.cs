using UnityEngine;

public class ThrusterComponent : ShipComponent
{
    [SerializeField] private float _forwardThrust = 4.0f;
    [SerializeField] private float _backwardThrust = 0.0f;
    private KeyCode _forwardKey = KeyCode.UpArrow;
    private KeyCode _backwardKey = KeyCode.DownArrow;
    
    public Sprite thrusterOnSprite;
    public Sprite thrusterOffSprite;

    public override void Start()
    {
        base.Start();
        connections[0] = false;
        connections[1] = false;
        connections[3] = false;
    }

    public override void Initialize(Ship parentShip)
    {
        base.Initialize(parentShip);
        float rotation = transform.localRotation.eulerAngles.z;
        if (AngleEqual(rotation, 0))
        {
            _forwardKey = KeyCode.DownArrow;
            _backwardKey = KeyCode.UpArrow;
        }
        else if (AngleEqual(rotation, 180))
        {
            _forwardKey = KeyCode.UpArrow;
            _backwardKey = KeyCode.DownArrow;
        }
        else if (AngleEqual(rotation, 90))
        {
            _forwardKey = KeyCode.RightArrow;
            _backwardKey = KeyCode.LeftArrow;
        }
        else if (AngleEqual(rotation, 270))
        {
            _forwardKey = KeyCode.LeftArrow;
            _backwardKey = KeyCode.RightArrow;
        } else
        {
            print("Thruster rotation not supported.");
        }
    }
    private bool AngleEqual(float a, float b, float tolerance = 5.0f)
    {
        return Mathf.Abs(a - b) < tolerance;
    }
    protected override void Update()
    {
        base.Update();
        if (Frozen)
        {
            return;
        }
        if (Input.GetKey(_forwardKey))
        {
            ParentShip.Thrust(_forwardThrust, -transform.up, transform.position);
            sprite.sprite = thrusterOnSprite;
        }
        if (Input.GetKeyUp(_forwardKey))
        {
            sprite.sprite = thrusterOffSprite;
        }
        if (Input.GetKey(_backwardKey))
        {
            ParentShip.Thrust(_backwardThrust, transform.up, transform.position);
        }
    }
}