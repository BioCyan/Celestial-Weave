using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Attaches to checkpoint boundry */
[RequireComponent(typeof(BoxCollider))]
public class RespawnController : MonoBehaviour
{
    [SerializeField] public GameObject checkPointLocation;

    private void Start()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.transform.position = checkPointLocation.transform.position;
            other.GetComponent<PlayerStats>().takeDamage(50);
        }
    }
}
