using UnityEngine;

public class ShipComponent : MonoBehaviour
{
    public bool[] connections;
    public int direction;

    public float Mass = 1.0f;
    public bool Frozen = true;

    public const float maxHealth = 100;
    public float currentHealth;


    public SpriteRenderer sprite;

    [HideInInspector] public Ship ParentShip;

    public virtual void Start()
    {
        currentHealth = maxHealth;
        sprite = GetComponent<SpriteRenderer>();
        connections = new bool[]{ true, true, true, true};
    }

    public virtual void Initialize(Ship parentShip)
    {
        currentHealth = maxHealth;
        direction = Mathf.RoundToInt(transform.localEulerAngles.z) / 90;
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
        if (currentHealth <= 0)
        {
            ParentShip.RemoveComponent(GetComponent<ShipComponent>());
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);
    }

    private void OnDestroy()
    {
        if(ParentShip != null) ParentShip.IntegrityCheck();
    }

    public void DestroyBlock()
    {
        Destroy(gameObject);
    }

    public bool CanConnectUp()
    {
        return connections[direction % 4];
    }

    public bool CanConnectRight()
    {
        return connections[(direction + 1) % 4];
    }

    public bool CanConnectDown()
    {
        return connections[(direction + 2) % 4];
    }

    public bool CanConnectLeft()
    {
        return connections[(direction + 3) % 4];
    }
}