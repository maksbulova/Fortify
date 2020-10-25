using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{
    private ParticleSystem bullets;

    // Start is called before the first frame update
    void Start()
    {
        bullets = GetComponent<ParticleSystem>();
    }

    private void Shoot()
    {
        bullets.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
