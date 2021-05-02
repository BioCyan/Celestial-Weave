using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformAttach : MonoBehaviour
{
    public GameObject Player;
    public bool onPlatform = false;

    private Vector3 oldPosition;

    private void Start()
    {
        oldPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (onPlatform) {
			Player.transform.position += transform.position - oldPosition;
        }
		oldPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Player)
		{
            onPlatform = true;
			//Player.transform.parent = transform;
		}
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == Player)
		{
            onPlatform = false;
			//Player.transform.parent = null;
		}
    }
}
