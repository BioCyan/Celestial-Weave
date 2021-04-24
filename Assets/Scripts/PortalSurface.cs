using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSurface : MonoBehaviour
{
	[SerializeField] public GameObject portalPrefab;
	[SerializeField] public float portalSeparation = 0.01f;

	void Start() {
	}

	public GameObject placePortal(Vector3 target, GameObject otherPortal) {
		float radius = portalPrefab.GetComponent<PortalScript>().radius;
		Vector3 diff = target - transform.position;
		float x = Vector3.Dot(diff, transform.right);
		float y = Vector3.Dot(diff, transform.up);
		float sizeX = transform.localScale.x / 2;
		float sizeY = transform.localScale.y / 2;
		if (Mathf.Abs(x) > sizeX - radius) {
			x = Mathf.Sign(x) * (sizeX - radius);
		}
		if (Mathf.Abs(y) > sizeY - radius) {
			y = Mathf.Sign(y) * (sizeY - radius);
		}
		Vector3 pos = transform.position;
		pos += transform.right * x;
		pos += transform.up * y;
		pos -= transform.forward * portalSeparation;

		GameObject portal = GameObject.Instantiate(portalPrefab, pos, transform.rotation);
		portal.GetComponent<PortalScript>().surface = gameObject;
		return portal;
	}
}
