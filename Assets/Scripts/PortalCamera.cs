using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour {
	[SerializeField] public GameObject cameraPrefab;
	[SerializeField] public int portalDepth = 1;

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
		leftPortal = GameObject.FindGameObjectWithTag("Left Portal");
		rightPortal = GameObject.FindGameObjectWithTag("Right Portal");

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

	void OnPreRender() {
		Material leftMaterial = leftPortal.GetComponent<Renderer>().material;
		for (int i = portalDepth - 1; i >= 0; i--) {
			leftCameras[i].GetComponent<Camera>().Render();

			leftMaterial.mainTexture = leftTextures[i];
		}

		Material rightMaterial = rightPortal.GetComponent<Renderer>().material;
		for (int i = portalDepth - 1; i >= 0; i--) {
			rightCameras[i].GetComponent<Camera>().Render();

			rightMaterial.mainTexture = rightTextures[i];
		}
		RenderTexture.active = null;
	}
}
