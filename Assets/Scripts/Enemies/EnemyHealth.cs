using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health = 10f;
    public GameObject[] drops;

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
        HitFXSingleton.Instance.SpawnDeathFX(transform.position);
        if (drops.Length > 0)
        {
            Instantiate(drops[Random.Range(0, drops.Length)], transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
