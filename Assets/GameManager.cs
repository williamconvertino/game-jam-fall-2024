using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private Entity[] _entities;
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
