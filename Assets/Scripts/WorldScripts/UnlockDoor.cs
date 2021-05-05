using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class UnlockDoor : MonoBehaviour
{
    [SerializeField] private GameObject KeyObject;
    [SerializeField] private GameObject DoorObject;

    private float keyResetDistance;
    private Vector3 startScale;
    private Vector3 tempScale;
    private Vector3 keyStartLocation;
    public Vector3 targetScale = new Vector3(0f,0f,0f);
    public bool invertedScale = false;
    public bool instantScale = false;
    public float resetDistanceModifier = 1.5f;  // This distance multiplied by the start separation between the cube and plate

    private void Start()
    {
        //Call the function giving it a target scale (Vector3) and a duration (float).
        //ScaleToTarget(new Vector3(2.0f, 0f, 1f), 2.5f);
        keyStartLocation = KeyObject.transform.position;
        startScale = DoorObject.transform.localScale;
        keyResetDistance = resetDistanceModifier * (keyStartLocation - gameObject.transform.position).magnitude;
        gameObject.GetComponent<BoxCollider>().isTrigger = true;
        if (invertedScale)  // Swap Vectors
        {
            tempScale = startScale;
            startScale = targetScale;   // startScale becomes <0, 0, 0>
            targetScale = tempScale;    // targetScale becomes <x, y, z>
            DoorObject.transform.localScale = startScale;
        }
    }

    private void FixedUpdate()
    {
        // Reset key if out of bounds
        float curDistance = (gameObject.transform.position - KeyObject.transform.position).magnitude;
        if (curDistance > keyResetDistance)
            ResetKeyPosition();
    }
    void ResetKeyPosition()
    {
        // play sound
        // play effect
        KeyObject.transform.position = keyStartLocation;
        KeyObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OPEN SESAME");
        if (other.gameObject == KeyObject || other.gameObject.tag == "Key")
        {
            DoorObject.GetComponent<AudioSource>().Play();
            StartCoroutine(ScaleToTargetCoroutine(targetScale, 2.5f));
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if((other.gameObject == KeyObject || other.gameObject.tag == "Key") && instantScale)
            DoorObject.transform.localScale = targetScale;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == KeyObject || other.gameObject.tag == "Key")
            StartCoroutine(ScaleToTargetCoroutine(startScale, 2.5f));
    }

    private IEnumerator ScaleToTargetCoroutine(Vector3 targetScale, float duration)
    {
        Vector3 startScale = DoorObject.transform.localScale;  // Initial scale of door
        if (instantScale)
            DoorObject.transform.localScale = targetScale;

        else
        {
            float timer = 0.0f;                                     // Initiate timer
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = timer / duration;
                //smoother step algorithm
                t = t * t * t * (t * (6f * t - 15f) + 10f);
                DoorObject.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
                yield return null;
            }
        }
        yield return null;
    }
}
