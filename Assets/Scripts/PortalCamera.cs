using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour {
	[SerializeField] public GameObject cameraPrefab;
	[SerializeField] public int portalDepth = 8;

	public GameObject[] leftCameras;
	public GameObject[] rightCameras;
	public GameObject leftPortal;
	public GameObject rightPortal;

	private RenderTexture[] leftTextures;
	private RenderTexture[] rightTextures;

	void Start() {
		leftCameras = new GameObject[portalDepth];
		leftTextures = new RenderTexture[portalDepth];
		for (int i = 0; i < portalDepth; i++) {
			leftTextures[i] = new RenderTexture(Screen.width, Screen.height, 24);

			leftCameras[i] = Object.Instantiate(cameraPrefab);
			leftCameras[i].name = "Left Portal Camera " + (i + 1);
			leftCameras[i].GetComponent<Camera>().targetTexture = leftTextures[i];
		}

		rightCameras = new GameObject[portalDepth];
		rightTextures = new RenderTexture[portalDepth];
		for (int i = 0; i < portalDepth; i++) {
			rightTextures[i] = new RenderTexture(Screen.width, Screen.height, 24);

			rightCameras[i] = Object.Instantiate(cameraPrefab);
			rightCameras[i].name = "Right Portal Camera " + (i + 1);
			rightCameras[i].GetComponent<Camera>().targetTexture = rightTextures[i];
		}
	}

	void Update() {
		if (Input.GetButtonDown("Fire1")) {
			leftPortal = ShootPortal(leftPortal, rightPortal);
			if (leftPortal != null) {
				InitLeftPortal();
			}
		}
		if (Input.GetButtonDown("Fire2")) {
			rightPortal = ShootPortal(rightPortal, leftPortal);
			if (rightPortal != null) {
				InitRightPortal();
			}
		}

		if (Input.GetKeyDown(KeyCode.Q)) {
			Destroy(leftPortal);
			Destroy(rightPortal);
		}
	}

	GameObject ShootPortal(GameObject oldPortal, GameObject otherPortal) {
		RaycastHit hitInfo;
		LayerMask mask = LayerMask.GetMask("Default");
		if (Physics.Raycast(transform.position, transform.forward, out hitInfo, Mathf.Infinity, mask)) {
			PortalSurface surface = hitInfo.collider.gameObject.GetComponent<PortalSurface>();
			if (surface != null) {
				GameObject portal = surface.PlacePortal(hitInfo.point, otherPortal);
				if (portal != null) {
					if (oldPortal != null) {
						GameObject.Destroy(oldPortal);
					}
					return portal;
				}
			}
		}
		return oldPortal;
	}

	void InitLeftPortal() {
		PortalScript script = leftPortal.GetComponent<PortalScript>();
		script.otherPortal = rightPortal;
		if (rightPortal != null) {
			rightPortal.GetComponent<PortalScript>().otherPortal = leftPortal;
		}
		script.entrantLayer = 8;
		script.colliderLayer = 10;
	}

	void InitRightPortal() {
		PortalScript script = rightPortal.GetComponent<PortalScript>();
		script.GetComponent<PortalScript>().otherPortal = leftPortal;
		if (leftPortal != null) {
			leftPortal.GetComponent<PortalScript>().otherPortal = rightPortal;
		}
		script.entrantLayer = 9;
		script.colliderLayer = 11;
	}

	void OnPreRender() {
		if (leftPortal == null || rightPortal == null) {
			return;
		}
		UpdateCameras();

		MeshRenderer leftBackupMesh = leftPortal.GetComponent<PortalScript>().backupMesh;
		MeshRenderer rightBackupMesh = rightPortal.GetComponent<PortalScript>().backupMesh;

		float epsilon = 0.05f;
		Plane leftPlane = new Plane(-leftPortal.transform.forward, leftPortal.transform.position);
		Vector4 leftPlaneVector = new Vector4(leftPlane.normal.x, leftPlane.normal.y, leftPlane.normal.z, leftPlane.distance + epsilon);
		Plane rightPlane = new Plane(-rightPortal.transform.forward, rightPortal.transform.position);
		Vector4 rightPlaneVector = new Vector4(rightPlane.normal.x, rightPlane.normal.y, rightPlane.normal.z, rightPlane.distance + epsilon);

		Material leftMaterial = leftPortal.GetComponent<Renderer>().material;
		leftPortal.GetComponent<PortalScript>().UpdateEntrants();
		Shader.SetGlobalVector("globalPlane", rightPlaneVector);
		for (int i = portalDepth - 1; i >= 0; i--) {
			leftCameras[i].GetComponent<Camera>().enabled = true;
			leftCameras[i].GetComponent<Camera>().Render();
			leftCameras[i].GetComponent<Camera>().enabled = false;

			leftMaterial.mainTexture = leftTextures[i];
		}

		Material rightMaterial = rightPortal.GetComponent<Renderer>().material;
		rightPortal.GetComponent<PortalScript>().UpdateEntrants();
		Shader.SetGlobalVector("globalPlane", leftPlaneVector);
		for (int i = portalDepth - 1; i >= 0; i--) {
			leftCameras[i].GetComponent<Camera>().enabled = true;
			rightCameras[i].GetComponent<Camera>().Render();
			leftCameras[i].GetComponent<Camera>().enabled = false;

			rightMaterial.mainTexture = rightTextures[i];
		}

		Shader.SetGlobalVector("globalPlane", Vector4.zero);
		RenderTexture.active = null;

		leftBackupMesh.material.mainTexture = leftMaterial.mainTexture;
		rightBackupMesh.material.mainTexture = rightMaterial.mainTexture;
		leftBackupMesh.enabled = leftPortal.GetComponent<PortalScript>().CameraNeedsBackup(transform.position);
		rightBackupMesh.enabled = rightPortal.GetComponent<PortalScript>().CameraNeedsBackup(transform.position);
	}

	void UpdateCameras() {
		Vector3 pos = this.transform.position;
		Quaternion rot = this.transform.rotation;
		Quaternion leftToRight = rightPortal.transform.rotation
			* Quaternion.Euler(0, 180, 0)
			* Quaternion.Inverse(leftPortal.transform.rotation);
		for (int i = 0; i < portalDepth; i++) {
			pos -= leftPortal.transform.position;
			pos = leftToRight * pos;
			pos += rightPortal.transform.position;
			rot = leftToRight * rot;

			leftCameras[i].transform.position = pos;
			leftCameras[i].transform.rotation = rot;
		}

		pos = this.transform.position;
		rot = this.transform.rotation;
		Quaternion rightToLeft = leftPortal.transform.rotation
			* Quaternion.Euler(0, 180, 0)
			* Quaternion.Inverse(rightPortal.transform.rotation);
		for (int i = 0; i < portalDepth; i++) {
			pos -= rightPortal.transform.position;
			pos = rightToLeft * pos;
			pos += leftPortal.transform.position;
			rot = rightToLeft * rot;

			rightCameras[i].transform.position = pos;
			rightCameras[i].transform.rotation = rot;
		}
	}
}
