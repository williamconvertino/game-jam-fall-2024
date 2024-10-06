using UnityEngine;

public class ShipComponent : MonoBehaviour
{
    public float Mass = 1.0f;
    public bool Frozen = true;

    public const float maxHealth = 100;
    public float currentHealth;


    public SpriteRenderer sprite;

    [HideInInspector] public Ship ParentShip;

    public void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public virtual void Initialize(Ship parentShip)
    {
        currentHealth = maxHealth;
        ParentShip = parentShip;
    }

    protected virtual void Update()
    {
        if (4 * currentHealth < maxHealth)
        {
            float percent = Mathf.Abs(0.5f - (2 * Time.time) % 1);
            sprite.color = Vector4.Lerp(Color.white, Color.red, 2 * percent);
        }
        else if (2 * currentHealth < maxHealth)
        {
            float percent = Mathf.Max(0, Mathf.Abs(1 - (2 * Time.time) % 2) - 0.5f);
            sprite.color = Vector4.Lerp(Color.white, Color.red, 1 * percent);
        }
        else
        {
            sprite.color = Color.white;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);

        if (currentHealth <= 0)
        {
            // this block is destroyed
        }
    }

}