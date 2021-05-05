using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        CurrentDifficulty = DifficultyLevel.Easy;
        Difficulty.text = "Difficulty : Easy "; 
    }
     
    public void Medium()
    {
        CurrentDifficulty = DifficultyLevel.Medium;
        Difficulty.text = "Difficulty : Medium ";
    }

    public void Hard()
    {
        CurrentDifficulty = DifficultyLevel.Hard;
        Difficulty.text = "Difficulty : Hard ";
    }
}
