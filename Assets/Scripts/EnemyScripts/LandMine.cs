using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMine : MonoBehaviour
{
    public float damage = 50f;
    public float proximityRange = 5f;
    private float lifeSpan = 8f;
    private float timer = 1.5f;
    private bool isAudio = false;
    private bool isDamaging = false;
    private GameObject player;
    private AudioSource audio;
    private float damageMulitplier = 1.0f;
    private DifficultyLevel curDifficulty;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        audio = GetComponent<AudioSource>();
        curDifficulty = OptionsController.CurrentDifficulty;
        SetDamageMultiplier();
    }

    void Update()
    {
        changeLevel();
        // If Player comes into range, it blows up
        if( Vector3.Distance(player.transform.position, transform.position) <= proximityRange )
        {
            blowUp();
            if( !(isAudio) )
            {
                isAudio = false;
                StartCoroutine(PlayAudio());
            }
            // If Player is still in range after timer(ie. 1.5 sec) then damage player
            timer -= Time.deltaTime;
            if( timer <= 0f )
            {
                if( Vector3.Distance(player.transform.position, transform.position) <= proximityRange && !(isDamaging) )
                {
                    damagePlayer();
                }
            }
        }
        //If landMine is set for to long then it disappears
        lifeSpan-= Time.deltaTime;
        if( lifeSpan <= 0f )
        {
            Destroy(gameObject);
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
            curDifficulty = OptionsController.CurrentDifficulty;
            SetDamageMultiplier();
        }
    }

    private void damagePlayer()
    {
        isDamaging = true;
        player.GetComponent<PlayerStats>().takeDamage(damage*damageMulitplier);
    }
    
    IEnumerator PlayAudio()
    {
        audio.Play();
        yield return new WaitForSeconds(5);
        isAudio = true;
    }

    public float takeDamage()
    {
        return damage;
    }

    public void blowUp()
    {
        //Disables red ball to show animation of bomb 
        transform.Find("LandMine").gameObject.SetActive(false);
        GetComponent<ParticleSystem>().Play();
        Destroy(gameObject,2f);
    }
}
