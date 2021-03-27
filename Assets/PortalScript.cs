using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScript : MonoBehaviour {
	[SerializeField] GameObject surface;
	[SerializeField] GameObject colliderZone;
	[SerializeField] GameObject colliderPrefab;
	public GameObject otherPortal = null;
	public int entrantLayer = 8;
	public int colliderLayer = 10;

	private HashSet<GameObject> entrants;

	void Start() {
		BoxCollider zone = colliderZone.GetComponent<BoxCollider>();
		Vector3 center = colliderZone.transform.TransformPoint(zone.center);
		Vector3 size = zone.size * 2;
		Collider[] colliders = Physics.OverlapBox(center, size / 2, transform.rotation);
		foreach (Collider collider in colliders) {
			if (collider is MeshCollider && collider.gameObject.GetComponent<Rigidbody>() == null) {
				if (collider.gameObject == surface) {
					continue;
				}

				Vector3 relativePos = collider.gameObject.transform.InverseTransformPoint(transform.position);
				Vector3 relativeDir = collider.gameObject.transform.InverseTransformDirection(-transform.forward);
				Plane plane = new Plane(relativeDir, relativePos);
				Mesh newMesh = ClipMesh(plane, ((MeshCollider)collider).sharedMesh);

				GameObject clippedCopy = Object.Instantiate(colliderPrefab, collider.gameObject.transform);
				clippedCopy.GetComponent<MeshCollider>().sharedMesh = newMesh;
				clippedCopy.layer = colliderLayer;
				clippedCopy.SetActive(true);
			}
		}

		entrants = new HashSet<GameObject>();
		gameObject.layer = entrantLayer;
	}

	void Update() {
		// Check for portal crossings
		Plane portalPlane = new Plane(transform.forward, transform.position);
		HashSet<GameObject> removals = new HashSet<GameObject>();
		foreach (GameObject entrant in entrants) {
			if (portalPlane.GetSide(entrant.transform.position)) {
				Quaternion portalToPortal = otherPortal.transform.rotation
					* Quaternion.Euler(0, 180, 0)
					* Quaternion.Inverse(transform.rotation);

				Vector3 pos = entrant.transform.position;
				pos -= transform.position;
				pos = portalToPortal * pos;
				pos += otherPortal.transform.position;

				Quaternion rot = entrant.transform.rotation;
				rot = portalToPortal * rot;

				entrant.transform.position = pos;
				entrant.transform.rotation = rot;
				removals.Add(entrant);
			}
		}
		foreach (GameObject removal in removals) {
			RemoveEntrant(removal);
		}

		// Add new entrants
		BoxCollider zone = GetComponent<BoxCollider>();
		Vector3 center = transform.TransformPoint(zone.center);
		Vector3 size = zone.size * 2;
		Collider[] colliders = Physics.OverlapBox(center, size / 2, transform.rotation);
		foreach (Collider collider in colliders) {
			GameObject entrant = collider.gameObject;
			if (entrant.GetComponent<Rigidbody>() != null && !entrants.Contains(entrant)) {
				entrants.Add(entrant);
				entrant.layer = entrantLayer;
			}
		}
	}

	void OnTriggerExit(Collider other) {
		if (entrants.Contains(other.gameObject)) {
			RemoveEntrant(other.gameObject);
		}
	}

	void RemoveEntrant(GameObject entrant) {
		entrants.Remove(entrant);
		if (entrant.layer == entrantLayer) {
			entrant.layer = 0;
		}
	}

	Mesh ClipMesh(Plane plane, Mesh oldMesh) {
		Vector3[] oldVertices = oldMesh.vertices;
		int[] oldTriangles = oldMesh.triangles;

		List<Vector3> points = new List<Vector3>();
		List<int> indices = new List<int>();
		for (int i = 0; i < oldTriangles.Length / 3; i++) {
			int count = 0;
			Vector3[] poly = new Vector3[4];
			for (int j = 0; j < 3; j++) {
				Vector3 point = oldVertices[oldTriangles[3 * i + j]];
				Vector3 otherPoint = oldVertices[oldTriangles[3 * i + (j + 1) % 3]];
				if (plane.GetSide(point)) {
					poly[count++] = point;
					if (!plane.GetSide(otherPoint)) {
						poly[count++] = IntersectEdge(plane, point, otherPoint);
					}
				} else if (plane.GetSide(otherPoint)) {
					poly[count++] = IntersectEdge(plane, otherPoint, point);
				}
			}
			if (count >= 3) {
				indices.Add(points.Count);
				points.Add(poly[0]);
				indices.Add(points.Count);
				points.Add(poly[1]);
				indices.Add(points.Count);
				points.Add(poly[2]);
			}
			if (count == 4) {
				indices.Add(points.Count);
				points.Add(poly[0]);
				indices.Add(points.Count);
				points.Add(poly[2]);
				indices.Add(points.Count);
				points.Add(poly[3]);
			}
		}

		Mesh newMesh = new Mesh();
		newMesh.vertices = points.ToArray();
		newMesh.triangles = indices.ToArray();
		return newMesh;
	}

	Vector3 IntersectEdge(Plane plane, Vector3 a, Vector3 b) {
		float distA = plane.GetDistanceToPoint(a);
		float distB = -plane.GetDistanceToPoint(b);
		float t = distA / (distA + distB);

		Vector3 result = (1 - t) * a + t * b;
		return result;
	}
}
