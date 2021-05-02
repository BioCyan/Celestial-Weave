using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScript : MonoBehaviour {
	[SerializeField] public GameObject surface;
	[SerializeField] public GameObject colliderZone;
	[SerializeField] public GameObject colliderPrefab;
	[SerializeField] public GameObject meshPrefab;
	[SerializeField] public MeshRenderer backupMesh;
	[SerializeField] public float radius = 1;
	public GameObject otherPortal = null;
	public int entrantLayer = 8;
	public int colliderLayer = 10;

	private Dictionary<GameObject, GameObject> entrants;
	private List<GameObject> clippedColliders;

	void Start() {
		BoxCollider zone = colliderZone.GetComponent<BoxCollider>();
		Vector3 center = colliderZone.transform.TransformPoint(zone.center);
		Vector3 size = zone.size * 3;
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

		entrants = new Dictionary<GameObject, GameObject>();
		gameObject.layer = entrantLayer;
	}

	void Update() {
		// Check for portal crossings
		Plane portalPlane = new Plane(transform.forward, transform.position);
		if (otherPortal != null) {
			HashSet<GameObject> removals = new HashSet<GameObject>();
			foreach (GameObject entrant in entrants.Keys) {
				if (portalPlane.GetSide(entrant.transform.position)) {
					portalTransform(entrant.transform);
					entrant.layer = otherPortal.GetComponent<PortalScript>().entrantLayer;
					removals.Add(entrant);
				}
			}
			foreach (GameObject removal in removals) {
				GameObject.Destroy(entrants[removal]);
				RemoveEntrant(removal);
				otherPortal.GetComponent<PortalScript>().AddEntrant(removal);
			}
		}

		// Add new entrants
		BoxCollider zone = GetComponent<BoxCollider>();
		Vector3 center = transform.TransformPoint(zone.center);
		Vector3 size = zone.size * 2;
		Collider[] colliders = Physics.OverlapBox(center, size / 2, transform.rotation);
		foreach (Collider collider in colliders) {
			GameObject entrant = collider.gameObject;
			if (entrant.GetComponent<Rigidbody>() != null && entrant.GetComponent<MeshFilter>() != null) {
				AddEntrant(entrant);
			}
		}
	}

	void OnTriggerExit(Collider other) {
		if (entrants.ContainsKey(other.gameObject)) {
			RemoveEntrant(other.gameObject);
		}
	}

	void AddEntrant(GameObject entrant) {
		if (!entrants.ContainsKey(entrant)) {
			GameObject copy = GameObject.Instantiate(meshPrefab);
			copy.transform.localScale = entrant.transform.localScale;
			copy.GetComponent<MeshFilter>().sharedMesh = entrant.GetComponent<MeshFilter>().sharedMesh;
			copy.GetComponent<Renderer>().sharedMaterial = entrant.GetComponent<Renderer>().sharedMaterial;
			copy.SetActive(true);
			entrants.Add(entrant, copy);
			entrant.layer = entrantLayer;
		}
	}

	void RemoveEntrant(GameObject entrant) {
		if (entrant == null) {
			return;
		}
		if (entrant.tag.Equals("Player")) {
			backupMesh.enabled = false;
		}
		GameObject.Destroy(entrants[entrant]);
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

	public void portalTransform(Transform frame) {
		Quaternion portalToPortal = otherPortal.transform.rotation
			* Quaternion.Euler(0, 180, 0)
			* Quaternion.Inverse(transform.rotation);

		Vector3 pos = frame.position;
		pos -= transform.position;
		pos = portalToPortal * pos;
		pos += otherPortal.transform.position;

		Quaternion rot = frame.rotation;
		rot = portalToPortal * rot;

		frame.position = pos;
		frame.rotation = rot;
	}

	public bool CameraNeedsBackup(Vector3 camPos) {
		Vector3 localPos = transform.InverseTransformPoint(camPos);
		BoxCollider zone = GetComponent<BoxCollider>();
		localPos -= zone.center;
		Vector3 halfSize = zone.size / 2;
		return localPos.x < halfSize.x
			&& localPos.x > -halfSize.x
			&& localPos.y < halfSize.y
			&& localPos.y > -halfSize.y
			&& localPos.z < halfSize.z
			&& localPos.z > -halfSize.z;
	}

	public void UpdateEntrants() {
		float epsilon = 0.05f;
		Plane plane = new Plane(-transform.forward, transform.position);
		Vector4 planeVector = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance + epsilon);
		Plane otherPlane = new Plane(-otherPortal.transform.forward, otherPortal.transform.position);
		Vector4 otherPlaneVector = new Vector4(otherPlane.normal.x, otherPlane.normal.y, otherPlane.normal.z, otherPlane.distance);

		foreach (GameObject entrant in entrants.Keys) {
			MaterialPropertyBlock block = new MaterialPropertyBlock();
			block.SetVector("localPlane", planeVector);
			entrant.GetComponent<Renderer>().SetPropertyBlock(block);

			GameObject copy = entrants[entrant];
			copy.transform.position = entrant.transform.position;
			copy.transform.rotation = entrant.transform.rotation;
			portalTransform(copy.transform);

			MaterialPropertyBlock copyBlock = new MaterialPropertyBlock();
			copyBlock.SetVector("localPlane", otherPlaneVector);
			copy.GetComponent<Renderer>().SetPropertyBlock(copyBlock);
		}
	}

	public void OnDestroy() {
		foreach (GameObject collider in clippedColliders) {
			GameObject.Destroy(collider);
		}

		HashSet<GameObject> removals = new HashSet<GameObject>();
		foreach (GameObject entrant in entrants.Keys) {
			removals.Add(entrant);
		}
		foreach (GameObject entrant in removals) {
			RemoveEntrant(entrant);
		}
	}
}
