using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    private NavMeshAgent agent;
    private Rigidbody rigidbody;

    public float health = 75f;
    public float damage = 85f;
    [SerializeField] private float speed = 3f;
    private float distance;
    private float minDistance = 10f;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // if player comes into distance than follow player
        distance = Vector3.Distance(player.transform.position, transform.position);
        if( distance <= minDistance )
        {
            transform.LookAt(player.transform.position);
            agent.destination = player.transform.position;
        }
        // if enemy health below 0 die
        if( health <= 0f )
        {
            enemyDead();
        }
    }

    private void suicideKill()
    {
        // if player comes into attack range 
        // attack player 
        // destroy enemy
    }

    private void enemyDead()
    {
        // do dead animation
        Destroy(gameObject);
    }
}

