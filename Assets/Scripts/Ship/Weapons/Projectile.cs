using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]

public class Projectile : MonoBehaviour
{
    [SerializeField] private int _damage = 1;
    [SerializeField] private float _speed = 2.0f;
    [SerializeField] private float _lifetime = 2.0f;
    private float _lifetimeTimer = 0.0f;
    private Rigidbody2D _rb2d;
    private string[] _tagsToIgnore;

    public void Initialize(Vector3 direction, Vector3 sourceVelocity, string[] tagsToIgnore)
    {
        _tagsToIgnore = tagsToIgnore;
        _rb2d = GetComponent<Rigidbody2D>();
        Vector3 velocity = direction.normalized * _speed + sourceVelocity;
        _rb2d.velocity = velocity;
    }

    private void Update()
    {
        _lifetimeTimer += Time.deltaTime;
        if (_lifetimeTimer >= _lifetime)
        {
            Destroy(gameObject);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {

        EnemyHealth enemy = collision.gameObject.GetComponent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.ApplyDamage(_damage);
            HitFXSingleton.Instance.SpawnHitFX(collision.GetContact(0).point);
        }
        else print("projectile collided with a non-enemy");

        Destroy(gameObject);
    }
}
