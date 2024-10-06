using UnityEngine;
public class Enemy : Entity
{
    public GameObject target;

    public float pursueRange;
    public float closeRange;
    public float acceleration;
    public float decceleration;
}