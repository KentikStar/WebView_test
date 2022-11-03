using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField]
    GameObject menu;

    [SerializeField]
    SpawnTetromino spawnTetromino;

    private float fallTime;

    private int score;
    public int Score{
        get{
            return score;
        }
        set{
            score = value;
            textScore.text = Convert.ToString(Score);
        }
    }

    [SerializeField]
    TextMeshProUGUI textScore;
    [SerializeField]
    TextMeshProUGUI textScoreEnd;

    void Start()
    {
        menu.SetActive(true);
        fallTime = TetrisBlock.fallTime;
    }

    public void StartGame(){        
        spawnTetromino.ClearChild();
        spawnTetromino.IninTetr();
        menu.SetActive(false);
        Score = 0;

        TetrisBlock.fallTime = fallTime;
    }

    public void EndGame(){
        menu.SetActive(true);
        textScoreEnd.text = textScore.text;
    }

    public void ExitGame(){
        Application.Quit();
    }
}
