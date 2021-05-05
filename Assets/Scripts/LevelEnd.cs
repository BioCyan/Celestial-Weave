using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
	[SerializeField] public int nextScene = 0;
	GameObject levelObject;
	Text levelNumText;

	private void Start()
    {
		levelNumText = GameObject.Find("LevelNumText").GetComponent<Text>();
		levelNumText.text = "" + (SceneManager.GetActiveScene().buildIndex +1);
		levelObject = GameObject.Find("Level");
		levelObject.SetActive(false);
	}
    public void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag == "Player") {
			levelObject.SetActive(true);
			
		}
	}

	IEnumerator loadNextScene() 
	{
		yield return new WaitForSeconds(3);
		SceneManager.LoadScene(nextScene);

	}
}
