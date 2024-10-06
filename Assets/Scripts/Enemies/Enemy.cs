using UnityEngine;
public abstract class Enemy : Entity
{
    [Header("Base Enemy Stats")]
    [SerializeField] protected int Health;
    [SerializeField] protected int Damage;
    [SerializeField] protected float Speed;
    [SerializeField] protected float PursueRange;

    protected Ship PlayerShip;
    protected Transform Target;
    protected virtual void Start()
    {
        PlayerShip = GameManager.Instance.PlayerShip;
        Target = PlayerShip.transform;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, PursueRange);
    }
}