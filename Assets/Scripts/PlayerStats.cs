using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float maxHealth = 100;
    public float health = 100;
//    [SerializeField] Text healthCounter;
    public float maxShield = 50;
    public float shield = 0;
//    [SerializeField] Text shieldCounter;
    public float maxEnergy = 100;
    public float energy = 100;
//    [SerializeField] Text energyCounter;
    [SerializeField] bool imune = false;
    [SerializeField] bool isDead = false;
    private float lastHit = 0f;
    private float imuneCoolDown = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        shield = maxShield;
        energy = maxEnergy;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Time.time - lastHit) > imuneCoolDown)
            imune = false;
        if ((health <= 0) && (isDead == false))
        {
            isDead = true;
            // Call gameOver();
        }
    }

    public void takeDamage(float damage)
    {
        if (imune == true)
        {
            Debug.Log("imune to damage");
        }
        else
        {
            if (shield > 0)
            {
                float newShield = shield - damage;
                if (newShield < 0)
                    newShield = 0;
                shield = newShield;
            }
            else
            {
                float newHealth = health - damage;
                if (newHealth < 0)
                    newHealth = 0;
                health = newHealth;
            }

            imune = true;
            lastHit = Time.time;
        }
    }

    public void healPlayer(float healVal)
    {
            float newHealth = health + healVal;
            if (newHealth > maxHealth)
                newHealth = maxHealth;
            health = newHealth;
    }

    public void rechargeShield(float rechargeVal)
    {
            float newShield = shield + rechargeVal;
            if (newShield > maxShield)
                 newShield = maxShield;
            shield = newShield;
    }

    public void rechargeEnergy(float rechargeVal)
    {
            float newEnergy = energy + rechargeVal;
            if (newEnergy > maxEnergy)
                newEnergy = maxEnergy;
            energy = newEnergy;
    }

}
