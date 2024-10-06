using UnityEngine;

public class Swarmer : Enemy
{
    protected override void Update()
    {
        if (Frozen)
        {
            return;
        }
        if (Vector2.Distance(Rb2d.position, Target.position) > PursueRange) return;
        Vector2 direction = (Vector2)Target.position - Rb2d.position;
        direction.Normalize();
        Rb2d.AddForceAtPosition(direction * Speed, Rb2d.position);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
    }
}