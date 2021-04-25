using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour {
	[SerializeField] public GameObject cameraPrefab;
	[SerializeField] public int portalDepth = 8;

	public GameObject[] leftCameras;
	public GameObject[] rightCameras;

	private RenderTexture[] leftTextures;
	private RenderTexture[] rightTextures;
	private GameObject leftPortal;
	private GameObject rightPortal;

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
	}

	GameObject ShootPortal(GameObject oldPortal, GameObject otherPortal) {
		RaycastHit hitInfo;
		if (Physics.Raycast(transform.position, transform.forward, out hitInfo)) {
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
		bool leftOldActive = leftBackupMesh.enabled;
		bool rightOldActive = rightBackupMesh.enabled;
		//leftBackupMesh.enabled = false;
		//rightBackupMesh.enabled = false;

		Plane leftPlane = new Plane(-leftPortal.transform.forward, leftPortal.transform.position);
		Plane rightPlane = new Plane(-rightPortal.transform.forward, rightPortal.transform.position);

		Material leftMaterial = leftPortal.GetComponent<Renderer>().material;
		Plane plane = rightPlane;
		Vector4 planeVector = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);
		for (int i = portalDepth - 1; i >= 0; i--) {
			Shader.SetGlobalVector("globalPlane", planeVector);
			leftCameras[i].GetComponent<Camera>().Render();

			leftMaterial.mainTexture = leftTextures[i];
		}

		Material rightMaterial = rightPortal.GetComponent<Renderer>().material;
		plane = leftPlane;
		planeVector = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);
		for (int i = portalDepth - 1; i >= 0; i--) {
			Shader.SetGlobalVector("globalPlane", planeVector);
			rightCameras[i].GetComponent<Camera>().Render();

			rightMaterial.mainTexture = rightTextures[i];
		}

		Shader.SetGlobalVector("globalPlane", Vector4.zero);
		RenderTexture.active = null;

		leftBackupMesh.material.mainTexture = leftMaterial.mainTexture;
		rightBackupMesh.material.mainTexture = rightMaterial.mainTexture;
		leftBackupMesh.enabled = leftOldActive;
		rightBackupMesh.enabled = rightOldActive;
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
