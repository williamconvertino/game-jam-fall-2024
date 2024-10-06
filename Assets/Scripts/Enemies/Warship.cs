using UnityEngine;

public class Warship : Enemy
{
    public float StopDistance = 1;
    public float RotateSpeed = 1;
    
    private Vector3 _targetLookDirection;

    private AutoCannon[] _autoCannons;

    protected override void Start()
    {
        base.Start();
        _autoCannons = GetComponentsInChildren<AutoCannon>();
        foreach (var cannon in _autoCannons)
        {
            cannon.Source = this;
        }
    }

    protected override void Update()
    {
        if (Frozen)
        {
            return;
        }
        if (Vector2.Distance(Rb2d.position, Target.position) > PursueRange) return;
        
        Vector2 directionToTarget = (transform.position - Target.position).normalized;
        
        if (Vector2.Distance(Rb2d.position, Target.position) > StopDistance)
        {
            _targetLookDirection = -directionToTarget.normalized;
        }
        else
        {
            Vector2 rightDirection = transform.right;
            Vector2 leftDirection = -transform.right;
        
            float angleToLeft = Vector2.SignedAngle(directionToTarget, leftDirection);
            float angleToRight = Vector2.SignedAngle(directionToTarget, rightDirection);
        
            if (angleToLeft < angleToRight)
            {
                _targetLookDirection = leftDirection.normalized;
            }
            else
            {
                _targetLookDirection = rightDirection.normalized;
            }
        }
        SlowRotateTowardsTarget();
        if (Vector2.Distance(Rb2d.position, Target.position) > StopDistance)
        {
            Rb2d.AddForce(-directionToTarget * Speed);
        }
        
        if (Vector2.Distance(Rb2d.position, Target.position) < StopDistance)
        {
            foreach (var cannon in _autoCannons)
            {
                cannon.TryFire();
            }
        }
    }
    
    public void SlowRotateTowardsTarget()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, _targetLookDirection), (RotateSpeed / 10.0f) * Time.deltaTime);
    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if (_targetLookDirection != null)
        {
            Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
            Gizmos.DrawRay(transform.position, _targetLookDirection);
        }
        
        Gizmos.color = new Color(1.0f, 1.0f, 0.0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, StopDistance);
    }
}