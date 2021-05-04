using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public float health = 50f;
    public float damage = 15f;

    [SerializeField] private GameObject laserTip;
    [SerializeField] private LineRenderer laser;
    private Animator anim;
    private AudioSource audio;
    private GameObject player;

    private float sightTime = 3f;
    private float laserRange = 8f;
    private bool isAudio = false;

    void Awake()
    {
        if( !laserTip )
            laserTip = GameObject.Find("LaserTip");
        if( !laser )
            laser = GetComponent<LineRenderer>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.Play("IdleBattle");
        if( health <= 0f )
        {
            Dead();
        }
        // If player comes into range then lock on
        if( Vector3.Distance(player.transform.position, transform.position) <= laserRange )
        {
            LockOnTarget();
            sightTime -= Time.deltaTime;
            // If target is within range for more than sightTime(3 sec) then start shooting laser
            if( sightTime <= 0f )
            {
                ShootLaser();
                // if theres no audio playing currently then play audio 
                if( !(isAudio) )
                {
                    isAudio = true;
                    StartCoroutine(PlayAudio());
                }
            }

        } else // player is outta range
        {
            // Reset sight time, set audio to end and disable the laser
            sightTime = 3f;
            isAudio = false;
            laser.enabled = false;            
        }

    }

    IEnumerator PlayAudio()
    {
        audio.Play();
        yield return new WaitForSeconds(6);
        isAudio = false;
    }

    private void LockOnTarget()
    {
        Vector3 dir = player.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    private void ShootLaser()
    {
        laser.enabled = true;
        audio.enabled = true;
        laser.SetPosition(0, laserTip.transform.position);
        laser.SetPosition(1, player.transform.position);
    }

    private void Dead()
    {
        anim.Play("Die");
        Destroy(gameObject, 1.5f);
    }

}
