using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    [SerializeField] public float health = 100f;
    [SerializeField] public float damage = 25f;
    [SerializeField] GameObject player;

    public void ReceivedDamage(float damage)
    {
        health -= damage;
        if( health <= 0 )
            Destroy(gameObject);
    }

    public void ReturnDamage()
    {
        player.GetComponent<PlayerStats>().takeDamage(damage);
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
