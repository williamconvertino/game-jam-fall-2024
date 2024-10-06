using UnityEngine;

public class Warship : Enemy
{
    public float StopDistance;
    
    private Vector3 _targetLookDirection;
    
    protected override void Update()
    {
        if (Frozen)
        {
            return;
        }
        if (Vector2.Distance(Rb2d.position, Target.position) > PursueRange) return;
        
        Vector2 directionToTarget = (transform.position - Target.position).normalized;
        Vector2 rightDirection = transform.right;
        Vector2 leftDirection = -transform.right;

    }
}