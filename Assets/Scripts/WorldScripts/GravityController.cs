using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour
{
    public Vector3 gravityForce = new Vector3(0f, -9.8f, 0f);

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("JUMP");
        other.gameObject.GetComponent<Rigidbody>().AddForce(gravityForce);
    }
}
