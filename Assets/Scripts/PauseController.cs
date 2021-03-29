using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{
	[SerializeField] MenuButtonController pauseMenu;
	public bool paused = false;

	void Update() {
		if (Input.GetKeyDown(KeyCode.P)) {
			TogglePause();
		}
	}

	public void TogglePause() {
		paused = !paused;
		if (paused) {
			pauseMenu.gameObject.SetActive(true);
			pauseMenu.PauseGame();
		} else {
			pauseMenu.ResumeGame();
			pauseMenu.gameObject.SetActive(false);
		}
	}
}
