using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSurface : MonoBehaviour
{
	[SerializeField] public GameObject portalPrefab;
	[SerializeField] public float portalSeparation = 0.01f;

	void Start() {
	}

	public GameObject PlacePortal(Vector3 target, GameObject otherPortal) {
		float radius = portalPrefab.GetComponent<PortalScript>().radius;
		float sizeX = transform.localScale.x / 2;
		float sizeY = transform.localScale.y / 2;

		Vector3 diff = target - transform.position;
		float x = Vector3.Dot(diff, transform.right);
		float y = Vector3.Dot(diff, transform.up);
		if (Mathf.Abs(x) > sizeX - radius) {
			x = Mathf.Sign(x) * (sizeX - radius);
		}
		if (Mathf.Abs(y) > sizeY - radius) {
			y = Mathf.Sign(y) * (sizeY - radius);
		}

		if (otherPortal != null && otherPortal.GetComponent<PortalScript>().surface == gameObject) {
			Vector3 otherDiff = otherPortal.transform.position - transform.position;
			float otherX = Vector3.Dot(otherDiff, transform.right);
			float otherY = Vector3.Dot(otherDiff, transform.up);
			if (Mathf.Abs(x - otherX) < 2 * radius && Mathf.Abs(y - otherY) < 2 * radius) {
				GameObject.Destroy(otherPortal);
			}
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

