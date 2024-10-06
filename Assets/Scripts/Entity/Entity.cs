using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Entity : MonoBehaviour
{
    public bool Frozen = true;
    private bool _wasFrozen = false;
    
    [HideInInspector] public Rigidbody2D Rb2d;

    private Vector3 _storedVelocity;

    protected void Awake()
    {
        Rb2d = GetComponent<Rigidbody2D>();
        Rb2d.drag = GameManager.Instance.BaseRb2d.drag;
        Rb2d.angularDrag = GameManager.Instance.BaseRb2d.angularDrag;
        Rb2d.gravityScale = GameManager.Instance.BaseRb2d.gravityScale;
    }

    protected void Update()
    {
        if (Frozen && !_wasFrozen)
        {
            _wasFrozen = true;
            Freeze();
        }
        else if (!Frozen && _wasFrozen)
        {
            _wasFrozen = false;
            Unfreeze();
        }
        if (!Frozen)
        {
            _storedVelocity = Rb2d.velocity;
        }
    }
    
    public virtual void Freeze()
    {
        Frozen = true;
        Rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
    }
    
    public virtual void Unfreeze()
    {
        Frozen = false;
        Rb2d.constraints = RigidbodyConstraints2D.None;
        Rb2d.velocity = _storedVelocity;
    }
}