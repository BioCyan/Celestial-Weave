using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    [SerializeField] public float health = 100f;
    [SerializeField] public float damage = 25f;
    Player player;

    public void ReceivedDamage(float damage)
    {
        health -= damage;
        if( health <= 0 )
            Destroy(gameObject);
    }

    public void ReturnDamage()
    {
        player.takeDamage(damage);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
