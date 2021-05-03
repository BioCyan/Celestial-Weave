using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class UnlockDoor : MonoBehaviour
{
    [SerializeField] private GameObject KeyObject;
    [SerializeField] private GameObject DoorObject;

    private Vector3 startScale;
    private Vector3 tempScale;
    public Vector3 targetScale = new Vector3(0f,0f,0f);

    public bool invertedScale = false;

    private void Start()
    {
        //Call the function giving it a target scale (Vector3) and a duration (float).
        //ScaleToTarget(new Vector3(2.0f, 0f, 1f), 2.5f);
        startScale = DoorObject.transform.localScale;
        gameObject.GetComponent<BoxCollider>().isTrigger = true;
        if (invertedScale)  // Swap Vectors
        {
            tempScale = startScale;
            startScale = targetScale;   // startScale becomes <0, 0, 0>
            targetScale = tempScale;    // targetScale becomes <x, y, z>
            DoorObject.transform.localScale = startScale;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OPEN SESAME");
        if(other.gameObject == KeyObject)
            StartCoroutine(ScaleToTargetCoroutine(targetScale, 2.5f));
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == KeyObject)
            StartCoroutine(ScaleToTargetCoroutine(startScale, 2.5f));
    }

    private IEnumerator ScaleToTargetCoroutine(Vector3 targetScale, float duration)
    {
        Vector3 startScale = DoorObject.transform.localScale;  // Initial scale of door
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
        yield return null;
    }
}
