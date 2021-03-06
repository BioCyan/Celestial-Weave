using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum DifficultyLevel { Easy, Medium, Hard };

public class OptionsController : MonoBehaviour
{
    [SerializeField] Text Difficulty;
    public static DifficultyLevel CurrentDifficulty = DifficultyLevel.Hard;

    private void Start()
    {
        Difficulty.text = "Difficulty : Easy ";
    }
    public void Easy()
    {
		PlayerPrefs.SetInt("Difficulty", 1);
        CurrentDifficulty = DifficultyLevel.Easy;
        Difficulty.text = "Difficulty : Easy "; 
    }
     
    public void Medium()
    {
		PlayerPrefs.SetInt("Difficulty", 2);
        CurrentDifficulty = DifficultyLevel.Medium;
        Difficulty.text = "Difficulty : Medium ";
    }

    public void Hard()
    {
		PlayerPrefs.SetInt("Difficulty", 2);
        CurrentDifficulty = DifficultyLevel.Hard;
        Difficulty.text = "Difficulty : Hard ";
    }

    public void level1() 
    {
        SceneManager.LoadScene("Level_0");
    }

    public void level2()
    {
        SceneManager.LoadScene("Level_1");
    }

    public void level3()
    {
        SceneManager.LoadScene("Level_2");
    }

}
