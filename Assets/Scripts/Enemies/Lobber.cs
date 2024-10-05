using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Lobber : MonoBehaviour
{
    public GameObject target;

    public float pursueRange;
    public float closeRange;
    public float acceleration;
    public float decceleration;

    public float maxVeloToFire;
    public float rateOfFire;
    private float fireCooldown = 0;

    public float bulletVelo;

    public GameObject bullet_prefab;

    private Rigidbody2D rb;

    [SerializeField] private string[] _tagsToIgnore;

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
        fireCooldown -= Time.deltaTime;

        Vector2 toTarget = (target.transform.position - transform.position);

        // If out of range, just slow to a halt
        if (toTarget.magnitude > pursueRange)
        {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, decceleration * Time.deltaTime);
            return;
        }

        // If in range
        if (rb.velocity.magnitude <= maxVeloToFire && fireCooldown <= 0)
        {
            fireCooldown = rateOfFire;
            GameObject projectile = Instantiate(bullet_prefab, transform.position, Quaternion.identity);
            projectile.GetComponent<Rigidbody2D>().velocity = bulletVelo * toTarget.normalized;
        }


        // Accelerate towards target
        if (toTarget.magnitude > closeRange)
        {
            rb.AddForce(toTarget * acceleration);
            return;
        }

        // Slow down if already close to target
        rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, decceleration * Time.deltaTime);
    }
}
