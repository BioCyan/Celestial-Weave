using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public float health = 50f;
    public float damage = 15f;

    [SerializeField] private GameObject laserTip;
    [SerializeField] private LineRenderer laser;
    private GameObject player;

    private float sightTime = 3f;
    private float laserRange = 8f;

    void Awake()
    {
        if( !laserTip )
            laserTip = GameObject.Find("LaserTip");
        if( !laser )
            laser = GetComponent<LineRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // If player comes into range then lock on
        if( Vector3.Distance(player.transform.position, transform.position) <= laserRange )
        {
            LockOnTarget();
            sightTime -= Time.deltaTime;
            // If target is within range for more than sightTime(3 sec) then start shooting laser
            if( sightTime <= 0f )
                ShootLaser();
        } else // player is outta range
        {
            sightTime = 3f;
            laser.enabled = false;
        }

    }

    private void LockOnTarget()
    {
        Vector3 dir = player.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    private void ShootLaser()
    {
        laser.enabled = true;
        laser.SetPosition(0, laserTip.transform.position);
        laser.SetPosition(1, player.transform.position);
    }
}
