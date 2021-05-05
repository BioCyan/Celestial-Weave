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
	Text timeText;

	private void Start()
	{
		levelNumText = GameObject.Find("LevelNumText").GetComponent<Text>();
		levelNumText.text = "Level " + SceneManager.GetActiveScene().buildIndex + " Complete";
		levelObject = GameObject.Find("Level");
		timeText = GameObject.Find("TimeText").GetComponent<Text>();
		levelObject.SetActive(false);
	}

	public void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag == "Player") {
			levelObject.SetActive(true);
			timeText.text = "Time: " + GetTime(Time.time);
			collision.gameObject.GetComponent<PlayerMovement>().enabled = false;
			NextLevel();
		}
	}

	// answers.unity.com/questions/45676/making-a-timer-0000-minutes-and-seconds.html
	string GetTime(float t)
	{
		string minutes = Mathf.Floor(t / 60).ToString("00");
		string seconds = (t % 60).ToString("00");
		return string.Format("{0}:{1}", minutes, seconds);
	}

	public void NextLevel()
	{
		if (SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings - 1) {
			nextScene = SceneManager.GetActiveScene().buildIndex + 1;
		} else {
			nextScene = 0;
		}
		Debug.Log("Nextscene: " + nextScene);
		StartCoroutine( loadNextScene() );
	}

	IEnumerator loadNextScene()
	{
		yield return new WaitForSeconds(4);
		SceneManager.LoadScene(nextScene);

	}
}
