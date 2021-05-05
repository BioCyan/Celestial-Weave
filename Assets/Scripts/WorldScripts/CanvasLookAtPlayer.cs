using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasLookAtPlayer : MonoBehaviour
{
    [SerializeField] private Transform playerCam;
    [SerializeField] private float viewDistance = 10f;
    void Start()
    {
        if (playerCam == null)
            playerCam = GameObject.FindGameObjectWithTag("Player").transform.Find("Main Camera");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float dis = (transform.position - playerCam.position).magnitude;
        if (dis < viewDistance)
        {
            GetComponent<Canvas>().enabled = true;
            transform.LookAt(2 * transform.position - playerCam.position);
        }
        else
            GetComponent<Canvas>().enabled = false;
    }
}
