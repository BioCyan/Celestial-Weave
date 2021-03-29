using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
	[SerializeField] public int nextScene = 0;

	public void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag == "Player") {
			SceneManager.LoadScene(nextScene);
		}
	}
}
