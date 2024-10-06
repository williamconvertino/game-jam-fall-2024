using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject _target;
    
    private Enemy[] _enemies;
    void Start()
    {
        _enemies = GetComponentsInChildren<Enemy>();
        foreach (var enemy in _enemies)
        {
            enemy.target = _target;
        }
    }
}
