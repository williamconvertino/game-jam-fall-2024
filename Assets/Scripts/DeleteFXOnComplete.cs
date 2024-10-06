using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteFXOnComplete : MonoBehaviour
{
    private float duration;
    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem parts = GetComponent<ParticleSystem>();
        duration = parts.duration + parts.startLifetime;
        Destroy(gameObject, duration);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
