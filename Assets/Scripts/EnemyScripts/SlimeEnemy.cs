﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    private NavMeshAgent agent;
    private Rigidbody rigidbody;
    private Animator anim;

    public float health = 75f;
    public float damage = 85f;
    public float attackRange = 3f;
    [SerializeField] private float speed = 3f;
    private float distance;
    private float minDistance = 10f;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.Play("Taunt");
        // if player comes into distance than follow player
        distance = Vector3.Distance(player.transform.position, transform.position);
        if( distance <= minDistance )
        {
            transform.LookAt(player.transform.position);
            agent.destination = player.transform.position;
            anim.Play("WalkFWD");
        }

        if( distance <= attackRange )
            suicideKill();
        // if enemy health below 0 die
        if( health <= 0f )
        {
            enemyDead();
        }
    }

    private void suicideKill()
    {
        // attack player 
        anim.Play("Attack02");
        // destroy enemy
        Destroy(gameObject, 1.5f);
    }

    public void getHit(float dmg)
    {
        anim.Play("GetHit");
        health -= dmg;
    }

    private void enemyDead()
    {
        // do dead animation
        anim.Play("Die");
        Destroy(gameObject);
    }
}

