using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Lobber : MonoBehaviour
{
    public GameObject target;

    public float pursueRange;
    public float fireRange;
    public float acceleration;
    public float decceleration;

    public GameObject bullet;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        // need to give it a player to target

        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Vector2 toTarget = (target.transform.position - transform.position);

        if (toTarget.magnitude > pursueRange)
        {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, decceleration * Time.deltaTime);
            return;
        }

        if (toTarget.magnitude > fireRange)
        {
            rb.AddForce(toTarget * acceleration);
            return;
        }

        // Slow down
        rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, decceleration * Time.deltaTime);

        // fire bullet!
    }
}
