using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public float health = 50f;
    public float shootRate = 50f;
    public float damage = 1f;

    [SerializeField] private GameObject laserTip;
    [SerializeField] private LineRenderer laser;
    private Animator anim;
    private AudioSource audio;
    private GameObject player;

    private float sightTime = 3f;
    private float laserRange = 10f;
    private float nextShootTime = 0f;
    private float damageMulitplier = 1.0f;
    private DifficultyLevel curDifficulty;
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
        curDifficulty = OptionsController.CurrentDifficulty;
        SetDamageMultiplier();
    }

    // Update is called once per frame
    void Update()
    {
        changeLevel();
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
                // Trying to damage player per fireRate
                if( Time.time > nextShootTime )
                {
                    damagePlayer();
                    nextShootTime = Time.time + 1/shootRate;
                }
                // if theres no audio playing currently when shooting laser then play audio 
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

    private void SetDamageMultiplier()
    {
        if( curDifficulty == DifficultyLevel.Easy )
            damageMulitplier = 1.0f;
        if( curDifficulty == DifficultyLevel.Medium )
            damageMulitplier = 2.0f;
        if( curDifficulty == DifficultyLevel.Hard )
            damageMulitplier = 3.0f;
    }

    private void changeLevel()
    {
        if( !(curDifficulty == OptionsController.CurrentDifficulty) )
        {
            Debug.Log("Changing level");
            curDifficulty = OptionsController.CurrentDifficulty;
            SetDamageMultiplier();
        }
        //Debug.Log("No level change");
    }

    private void damagePlayer()
    {
        player.GetComponent<PlayerStats>().takeDamage(damage*damageMulitplier);
    }

    IEnumerator PlayAudio()
    {
        // Plays audio for 6 seconds 
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
        //Enables laser lineRenderer and audio 
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
