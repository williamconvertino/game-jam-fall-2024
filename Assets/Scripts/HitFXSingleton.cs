using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitFXSingleton : MonoBehaviour
{
    private static HitFXSingleton _instance;

    public static HitFXSingleton Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public GameObject ProjectileHitFX;
    public GameObject DeathFX;

    public void SpawnHitFX(Vector3 position)
    {
        GameObject fx = Instantiate(ProjectileHitFX);
        fx.transform.position = position;
    }

    public void SpawnDeathFX(Vector3 position)
    {
        GameObject fx = Instantiate(DeathFX);
        fx.transform.position = position;
    }
}
