using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health = 10f;

    public void ApplyDamage(float damage)
    {
        health -= damage;
        if(health < damage)
        {
            BlowUp();
        }
    }

    private void BlowUp()
    {
        Destroy(gameObject);
    }
}
