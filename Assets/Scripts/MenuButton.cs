using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
	[SerializeField] MenuButtonController menuButtonController;
	[SerializeField] Animator animator;
	[SerializeField] AnimatorFunctions animatorFunctions;
	[SerializeField] int thisIndex;
	[SerializeField] bool clicked ;
	[SerializeField] PauseController pauseController;

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
					Time.timeScale = 1;
					menuButtonController.newGame();
				}
				else if (thisIndex == 1)
				{
					//pauseController.TogglePause();
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
