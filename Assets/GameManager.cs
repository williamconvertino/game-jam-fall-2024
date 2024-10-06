using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Ship PlayerShip;
    public Rigidbody2D BaseRb2d;
    private Entity[] _entities;
    
    public GameObject[] ActivateOnStart;
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        if (PlayerShip == null)
        {
            PlayerShip = GetComponentInChildren<Ship>();
        }
        
        if (BaseRb2d == null)
        {
            BaseRb2d = GetComponent<Rigidbody2D>();
        }
        
        BaseRb2d.simulated = false;

        foreach (var obj in ActivateOnStart)
        {
            obj.SetActive(true);
        }
    }
    void Start()
    {
        _entities = FindObjectsOfType<Entity>();
        Pause();
    }

    public void Pause()
    {
        foreach (var entity in _entities)
        {
            entity.Freeze();
        }   
    }
    
    public void Unpause()
    {
        foreach (var entity in _entities)
        {
            entity.Unfreeze();
        }
    }
}
