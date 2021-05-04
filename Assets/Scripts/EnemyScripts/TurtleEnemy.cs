using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleEnemy : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent agent;
    private Animator anim;
    private Vector3 spawnPos;
    private Quaternion spawnRot;
    public GameObject landMine;
    public GameObject player;

    public float health = 100f;
    [SerializeField] private float speed = 5f;
    private float roamRadius = 20f;
    private float timer = 5f;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Awake()
    {
        spawnPos = transform.position;
        spawnRot = transform.rotation;
    }

    void Update()
    {
        //Randomly walk around
        Patrolling();

        // Drop projectile every 5 seconds
        timer -= Time.deltaTime;
        if( timer <= 0f )
        {
            dropLandMine();
            timer = 5f;
        }
        // if enemy health below 0
        if( health <= 0 )
        {
            enemyDead();
        }
    }

    // Drops landMine
    public void dropLandMine()
    {
        Vector3 landMinePos = new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z);
        Instantiate(landMine, landMinePos, landMine.transform.rotation);
    }
    
    public void Patrolling()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += transform.position;
        UnityEngine.AI.NavMeshHit hit;
        UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, roamRadius, 1);
        Vector3 finalPos = hit.position;

        // If enemy roams to far from original location.
        if( Vector3.Distance(finalPos, spawnPos) > roamRadius )
            agent.destination = spawnPos;
        else
            agent.destination = finalPos * Time.deltaTime * speed;
        anim.Play("WalkFWD");
    }

    public void enemyDead()
    {
        //Start dead animation
        anim.Play("Die");
        Destroy(gameObject, 2f);
    }
}
