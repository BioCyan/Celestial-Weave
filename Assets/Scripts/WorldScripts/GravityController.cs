using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour
{
    public float height = 10f;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("JUMP");
        other.GetComponent<Rigidbody>().velocity = Vector3.zero;
        Vector3 upVec = transform.up;
        float force = Mathf.Sqrt(2 * height * 9.8f) * other.GetComponent<Rigidbody>().mass;
        other.gameObject.GetComponent<Rigidbody>().AddForce(force * upVec, ForceMode.Impulse);
        GetComponent<AudioSource>().Play();
        //Debug.Log(upVec.ToString() + " " + Vector3.up);
    }
}
