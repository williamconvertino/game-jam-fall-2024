using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Grabber : Enemy
{
    private Vector2 prev_error;
    
    private Rigidbody2D rb;

    [SerializeField] private string[] _tagsToIgnore;

    // Start is called before the first frame update
    void Start()
    {
        // need to give it a player to target

        rb = GetComponent<Rigidbody2D>();
    }
    

    void FixedUpdate()
    {
        if (Frozen) return;
        Vector2 error = (target.transform.position - transform.position);
        Vector2 error_derivative = error - prev_error;
        float kp = 1f;
        float kd = 1f;
        float dt = 1 / 60f;
        // Accelerate towards target
        rb.AddForce((kp * error) + (kd * error_derivative / dt));
        prev_error = error;
        return;

    }
}
