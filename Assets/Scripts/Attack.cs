using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{

    [SerializeField] Transform FPSCamera;
    [SerializeField] GameObject[] weapon;
    [SerializeField] private int currentWeapon = 0;
    [SerializeField] private GameObject impactEffect;
    private float nextAttackTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (!FPSCamera)
            FPSCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(FPSCamera.position, FPSCamera.forward, Color.red);
        if (Input.GetButton("Fire1") && Time.time > nextAttackTime)
        {
            playerAttack();
            nextAttackTime = Time.time + (1 / weapon[currentWeapon].GetComponent<Weapon>().weaponSpeed);
        }
        if (Input.GetKeyDown("1") && (currentWeapon != 0))
        {
            currentWeapon = 0;
            changeWeapon();
            Debug.Log("Sword");
        }

        if (Input.GetKeyDown("2") && (currentWeapon != 1))
        {
            currentWeapon = 1;
            changeWeapon();
            Debug.Log("Laser");
        }

        if (Input.GetKeyDown("3") && (currentWeapon != 2))
        {
            currentWeapon = 2;
            changeWeapon();
            Debug.Log("Orb");
        }

    }

    void playerAttack()
    {
        AudioSource s = weapon[currentWeapon].GetComponent<AudioSource>();
        if (s != null)
            s.Play();
        //weapon[currentWeapon].GetComponent<Weapon>().muzzleFlash.Play();
        if (Physics.Raycast(FPSCamera.position, FPSCamera.forward, out RaycastHit hitInfo, weapon[currentWeapon].GetComponent<Weapon>().weaponRange))
        {
            Debug.Log(hitInfo.point);

            Rigidbody rb = hitInfo.rigidbody;

         // if (rb != null)
         //       rb.GetComponent<Enemy>().takeDamage(weapon[currentWeapon].GetComponent<Weapon>().gunDamage);


         //   GameObject g = Instantiate(impactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
         //   Destroy(g, 1f);
        }
    }

    void changeWeapon()
    {
        for (int i = 0; i < weapon.Length; i++)
        {
            weapon[i].SetActive(false);
        }
        weapon[currentWeapon].SetActive(true);
    }
}
