using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour
{
    public float height = 10f;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("JUMP");
        float force = Mathf.Sqrt(2 * height * 9.8f) * other.GetComponent<Rigidbody>().mass;
        other.gameObject.GetComponent<Rigidbody>().AddForce(force * transform.up, ForceMode.Impulse);
        GetComponent<AudioSource>().Play();
    }
}
