using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
	[SerializeField] public int nextScene = 0;

	public void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag == "Player") {
			GameObject.Find("LevelNumText").GetComponent<Text>().text = ""+ SceneManager.GetActiveScene().buildIndex + 1 ;
			GameObject.Find("Level").SetActive(true);
			
		}
	}

	IEnumerator loadNextScene() 
	{
		yield return new WaitForSeconds(3);
		SceneManager.LoadScene(nextScene);

	}
}
