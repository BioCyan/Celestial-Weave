﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
	[SerializeField] MenuButtonController menuButtonController;
	[SerializeField] Animator animator;
	[SerializeField] AnimatorFunctions animatorFunctions;
	[SerializeField] int thisIndex;
	[SerializeField] bool clicked ;
	[SerializeField] PauseController pauseController;
	[SerializeField] GameObject optionsMenu;
	[SerializeField] GameObject mainMenu;

	private void Start()
    {
		if (SceneManager.GetActiveScene().buildIndex == 0) 
		{
			optionsMenu = GameObject.Find();
			mainMenu = GameObject.Find();

		}


	}
    // Update is called once per frame
    void Update()
	{
		if(menuButtonController.index == thisIndex)
		{
			animator.SetBool ("selected", true);
			if(Input.GetAxisRaw("Submit") == 1 || clicked )
			{
				animator.SetBool ("pressed", true);
				if (thisIndex == 0)
				{
					if (SceneManager.GetActiveScene().buildIndex == 0)
						SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
					else
						SceneManager.LoadScene(SceneManager.GetActiveScene().name);
					Time.timeScale = 1;
					//menuButtonController.newGame();
					
				}
				else if (thisIndex == 1)
				{
					//pauseController.TogglePause();
					if (SceneManager.GetActiveScene().buildIndex == 0)
						OpenOptions();
					else
						SceneManager.LoadScene("startMenu");
				}
				else if (thisIndex == 2)
				{
					menuButtonController.quit();
				}
			}
			else if (animator.GetBool ("pressed"))
			{
				animator.SetBool ("pressed", false);
				animatorFunctions.disableOnce = true;
			}
		}
		else
		{
			animator.SetBool ("selected", false);
		}

	}

	// NOTICE:: JOHN H!!!
	private void OpenOptions()
    {
		optionsMenu.SetActive(true);

		// Not sure how you had intented to set this set up.
		// Complete this function.
		// Should include:
		// Difficulty = {Easy, Medium, Hard}
		// LevelSelect = {1, 2, 3} by index where 0 = startmenu
	}

	public void mouseOver()
	{
		menuButtonController.index = thisIndex;
	}

	public void mouseClick()
	{
		clicked = true;
	}
	public void mouseExit()
	{
		clicked = false;
	}
}
