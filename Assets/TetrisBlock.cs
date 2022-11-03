using System.Text;
using System.Diagnostics;
using System;
using System.Security.AccessControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    [SerializeField]
    private Transform rotationPoint;
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    private float prviousTime;
    public static float fallTime = 0.8f;
    bool stationary = false;
    private Stopwatch stopwatch = new Stopwatch();
    
    private static int height = 25;
    private static int width = 10;
    private static Transform[,] grid = new Transform[width,height];

    UIController uIController;
    SpawnTetromino spawnTetromino;

    void Start()
    {
        uIController = FindObjectOfType<UIController>();
        spawnTetromino = FindObjectOfType<SpawnTetromino>();
    }


    void Update()
    {
        Test();
    }

    private void Test()
    {
        stationary = false;

        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            //FIRST touch
            if(touch.phase == TouchPhase.Began)
            {
                startTouchPosition = Input.GetTouch(0).position;
                stopwatch.Restart();
            }
            
            //Touch не сместился выше нормы (не двигается)
            if(CalculatedScatter(startTouchPosition, Input.GetTouch(0).position))
            {
                //Вражение блока, если убрал быстро палец 
                if(stopwatch.Elapsed.TotalSeconds < 0.3f)
                {
                    if(touch.phase == TouchPhase.Ended){
                            RotateBlock();
                    }
                }
                else //иначе ускорить падение
                    stationary = true;
            }
            else
            {
                //Движение в стороны
                if(touch.phase == TouchPhase.Ended){
                        endTouchPosition = Input.GetTouch(0).position;

                        if(endTouchPosition.x < startTouchPosition.x)
                            {
                                transform.position += Vector3.left;
                                if(!ValidMove())
                                    transform.position -= Vector3.left;
                            }

                        if(endTouchPosition.x > startTouchPosition.x)
                            {
                                transform.position += Vector3.right;
                                if(!ValidMove())
                                    transform.position -= Vector3.right;
                            }                        
                    }
            }
        }

        //Постоянное падение блока
        MoveVertical();
    }

    private void PlusScore(int factor){
        int score;
        switch(factor)
        {
            case 1:
                score = GetFactorScore(1);
            break;
            case 2:
                score = GetFactorScore(3);
            break;
            case 3:
                score = GetFactorScore(6);
            break;
            case 4:
                score = GetFactorScore(12);
            break;
            default:
                score = 0;
            break;
        }

        uIController.Score += score;
    }

    private int GetFactorScore(int value){
        return value * 100;
    }

    //Погрешность нажатия на точку
    private bool CalculatedScatter(Vector2 start, Vector2 end){
        Vector2 scatter = start - end;

        if(Math.Abs(scatter.x) <= 15 && Math.Abs(scatter.y) <= 15)
            return true;
        
        return false;
    }    

    //Постоянное падение
    private void MoveVertical()
    {
        if(Time.time - prviousTime >(stationary? fallTime/10 : fallTime))
        {
            transform.position += Vector3.down;            

            if(!ValidMove())
            {
                transform.position -= Vector3.down;

                AddToGrid();
                CheckForLines();
                this.enabled = false;

                BlockAboveBorder();
                    
            }

            prviousTime = Time.time;
        }       
    }

    //Поднялся выше границы, если нет - спавн нового
    private void BlockAboveBorder(){
        if(transform.position.y > height - 5)
        {
            uIController.EndGame();            
        }
        else
        {
            spawnTetromino.IninTetr();
            fallTime /= 1.003f;
            UnityEngine.Debug.Log(fallTime);
        }
    }

    //Вращение
    private void RotateBlock()
    {
        transform.RotateAround(transform.TransformPoint(rotationPoint.localPosition), Vector3.forward, 90);
        if(!ValidMove())
            transform.RotateAround(transform.TransformPoint(rotationPoint.localPosition), Vector3.forward, -90);
                
    }    

    //Сетка установленных блоков
    private void AddToGrid()
    {
        foreach(Transform children in transform)
        {
            if(children.tag == rotationPoint.tag)
                continue;

            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);

            grid[roundedX,roundedY] = children;
            
        }
    }

    //Проверка всех блоков
    private void CheckForLines()
    {
        int factor = 0;
        for(int i = height-1; i >= 0; i--)
        {
            if(HasLine(i))
            {
                factor++;
                DeleteLine(i);
                RowDown(i);
            }
        }

        PlusScore(factor);
    }

    private bool HasLine(int i)
    {
        for(int j = 0; j < width; j++)
        {
            if(grid[j,i] == null)
                return false;
        }

        return true;
    }

    private void DeleteLine(int i)
    {
        for(int j = 0; j < width; j++)
        {
            Destroy(grid[j,i].gameObject);
            grid[j,i] = null;
        }
    }

    private void RowDown(int i)
    {
        for(int y = i; y < height; y++)
        {
            for(int j = 0; j < width; j++)
            {
                if(grid[j,y] != null)
                {
                    grid[j, y - 1] = grid[j,y];
                    grid[j,y] = null;
                    grid[j,y-1].transform.position -= Vector3.up;
                }
            }
        }
    }

    //Ограничение движения
    bool ValidMove()
    {
        foreach(Transform children in transform)
        {
            if(children.tag == rotationPoint.tag)
                continue;

            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);

            if(roundedX < 0 || roundedX >= width || roundedY < 0 || roundedY >= height)
            {
                return false;
            }

            if(grid[roundedX,roundedY] != null)
                return false;
        }

        return true;
    }
}
