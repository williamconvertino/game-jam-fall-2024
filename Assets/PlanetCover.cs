using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetCover : MonoBehaviour
{
    public float ViewRadius = 50f; 
    private SpriteRenderer _spriteRenderer;
    private Ship _playerShip;
    

    public void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerShip = GameManager.Instance.PlayerShip;
    }

    public void Update()
    {
        if (Vector2.Distance(_playerShip.transform.position, transform.position) > ViewRadius)
        {
            _spriteRenderer.enabled = true;
        }
        else
        {
            _spriteRenderer.enabled = false;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, ViewRadius);
    }
}
