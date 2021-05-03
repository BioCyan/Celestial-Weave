using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnController : MonoBehaviour
{
    [SerializeField] public GameObject checkPointLocation;
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("RESET:" + other.name);
        other.transform.position = checkPointLocation.transform.position;
    }
}
