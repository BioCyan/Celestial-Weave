using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScript : MonoBehaviour {
	[SerializeField] public GameObject surface;
	[SerializeField] public GameObject colliderZone;
	[SerializeField] public GameObject colliderPrefab;
	[SerializeField] public MeshRenderer backupMesh;
	[SerializeField] public float radius = 1;
	public GameObject otherPortal = null;
	public int entrantLayer = 8;
	public int colliderLayer = 10;

	private HashSet<GameObject> entrants;
	private List<GameObject> clippedColliders;

	void Start() {
		BoxCollider zone = colliderZone.GetComponent<BoxCollider>();
		Vector3 center = colliderZone.transform.TransformPoint(zone.center);
		Vector3 size = zone.size * 2;
		Collider[] colliders = Physics.OverlapBox(center, size / 2, transform.rotation);
		clippedColliders = new List<GameObject>();
		foreach (Collider collider in colliders) {
			if (collider is MeshCollider && collider.gameObject.GetComponent<Rigidbody>() == null && collider.gameObject.layer != colliderLayer) {
				if (collider.gameObject == surface) {
					continue;
				}

				Vector3 relativeDir = collider.gameObject.transform.InverseTransformDirection(-transform.forward);
				Vector3 relativePos = collider.gameObject.transform.InverseTransformPoint(transform.position);
				Plane plane = new Plane(relativeDir, relativePos);
				Mesh newMesh = ClipMesh(plane, ((MeshCollider)collider).sharedMesh);

				GameObject clippedCopy = Object.Instantiate(colliderPrefab, collider.gameObject.transform);
				clippedCopy.GetComponent<MeshCollider>().sharedMesh = newMesh;
				clippedCopy.layer = colliderLayer;
				clippedCopy.SetActive(true);
				clippedColliders.Add(clippedCopy);
			}
		}
		Mesh holeMesh = DrillMesh(transform, surface.transform, surface.GetComponent<MeshCollider>().sharedMesh);
		GameObject holeCollider = Object.Instantiate(colliderPrefab, surface.transform);
		holeCollider.GetComponent<MeshCollider>().sharedMesh = holeMesh;
		holeCollider.layer = colliderLayer;
		holeCollider.SetActive(true);
		clippedColliders.Add(holeCollider);

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
				entrant.layer = otherPortal.GetComponent<PortalScript>().entrantLayer;
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

				if (entrant.tag.Equals("Player")) {
					backupMesh.enabled = true;
				}
			}
		}
	}

	void OnTriggerExit(Collider other) {
		if (entrants.Contains(other.gameObject)) {
			RemoveEntrant(other.gameObject);
		}
	}

	void RemoveEntrant(GameObject entrant) {
		if (entrant.tag.Equals("Player")) {
			backupMesh.enabled = false;
		}
		entrants.Remove(entrant);
		if (entrant.layer == entrantLayer) {
			entrant.layer = 0;
		}
	}

	Mesh DrillMesh(Transform portalFrame, Transform meshFrame, Mesh oldMesh) {
		Vector3 relativePos = meshFrame.InverseTransformPoint(transform.position);
		Vector3 relativeUp = meshFrame.InverseTransformDirection(portalFrame.up);
		Vector3 relativeRight = meshFrame.InverseTransformDirection(portalFrame.right);
		float xCenter = Vector3.Dot(relativePos, relativeRight);
		float yCenter = Vector3.Dot(relativePos, relativeUp);

		Mesh left = ClipMesh(new Plane(-relativeRight, xCenter - radius / meshFrame.localScale.x), oldMesh);
		Mesh right = ClipMesh(new Plane(relativeRight, -xCenter - radius / meshFrame.localScale.x), oldMesh);
		Mesh top = ClipMesh(new Plane(relativeUp, -yCenter - radius / meshFrame.localScale.y), oldMesh);
		Mesh bottom = ClipMesh(new Plane(-relativeUp, yCenter - radius / meshFrame.localScale.y), oldMesh);

		Mesh[] meshes = {left, right, top, bottom};
		return MergeMeshes(meshes);
	}

	Mesh MergeMeshes(IEnumerable<Mesh> meshes) {
		List<Vector3> vertices = new List<Vector3>();
		List<int> indices = new List<int>();

		int indexBase = 0;
		foreach (Mesh mesh in meshes) {
			Vector3[] oldVertices = mesh.vertices;
			int[] oldTriangles = mesh.triangles;

			foreach (Vector3 vertex in oldVertices) {
				vertices.Add(vertex);
			}
			foreach (int index in oldTriangles) {
				indices.Add(index + indexBase);
			}

			indexBase += oldVertices.Length;
		}

		Mesh newMesh = new Mesh();
		newMesh.vertices = vertices.ToArray();
		newMesh.triangles = indices.ToArray();
		return newMesh;
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

	public void Cleanup() {
		foreach (GameObject collider in clippedColliders) {
			GameObject.Destroy(collider);
		}
	}
}
