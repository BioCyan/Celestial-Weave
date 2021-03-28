using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtonController : MonoBehaviour
{

	// Use this for initialization
	public int index;
	[SerializeField] bool keyDown;
	[SerializeField] int maxIndex;
	public AudioSource audioSource;

	void Start()
	{
		audioSource = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update()
	{
		if(Input.GetAxis ("Vertical") != 0)
		{
			if(!keyDown)
			{
				if (Input.GetAxis ("Vertical") < 0)
				{
					if(index < maxIndex)
					{
						index++;
					}
					else
					{
						index = 0;
					}
				}
				else if(Input.GetAxis ("Vertical") > 0)
				{
					if(index > 0)
					{
						index--;
					}
					else
					{
						index = maxIndex;
					}
				}
				keyDown = true;
			}
		}
		else
		{
			keyDown = false;
		}
	}

	public void PauseGame()
	{
		Time.timeScale = 0;
	}

	public void ResumeGame()
	{
		Time.timeScale = 1;
	}
	public void quit()
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}

	public void newGame()
	{
		//PlayerPrefs.DeleteKey("PlayerHealth");
		//PlayerPrefs.DeleteKey("PlayerSpeed");
		//PlayerPrefs.DeleteKey("Level");
		SceneManager.LoadScene(1);
	}
	private void OnApplicationQuit()
	{
		//PlayerPrefs.DeleteKey("PlayerHealth");
		//PlayerPrefs.DeleteKey("PlayerSpeed");
		//PlayerPrefs.DeleteKey("Level");
	}

}
