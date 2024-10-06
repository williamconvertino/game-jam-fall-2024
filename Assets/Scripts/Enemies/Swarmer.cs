using UnityEngine;

public class Swarmer : Enemy
{
    protected void FixedUpdate()
    {
        Vector2 direction = (Vector2)Target.position - Rb2d.position;
        direction.Normalize();
        Rb2d.AddForceAtPosition(direction * Speed, Rb2d.position);
    }
}