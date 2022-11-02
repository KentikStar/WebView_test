using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    [SerializeField]
    private Vector3 rotationPoint;
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    private float prviousTime;
    private float fallTime = 0.8f;
    
    [SerializeField]
    private static int height = 25;
    [SerializeField]
    private static int width = 9;
    private static Transform[,] grid = new Transform[width,height];

    void Update()
    {
        //MOVE HORIZ
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            startTouchPosition = Input.GetTouch(0).position;
    
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended){
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
            //ROTATE
            if(endTouchPosition.y < startTouchPosition.y)
                {
                    transform.RotateAround(transform.TransformPoint(rotationPoint), Vector3.forward, 90);
                    if(!ValidMove())
                        transform.RotateAround(rotationPoint, Vector3.forward, -90);
                }
            
        }

         

        //MOVE VERT
        if(Time.time - prviousTime > fallTime)
        {
            transform.position += Vector3.down;            

            if(!ValidMove())
                {
                    transform.position -= Vector3.down;

                    AddToGrid();
                    CheckForLines();
                    this.enabled = false;

                    FindObjectOfType<SpawnTetromino>().IninTetr();
                }

            prviousTime = Time.time;
        }
    }

    private void AddToGrid()
    {
        foreach(Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);

            grid[roundedX,roundedY] = children; 
        }
    }

    private void CheckForLines()
    {
        for(int i = height-1; i >= 0; i--)
        {
            if(HasLine(i))
            {
                DeleteLine(i);
                RowDown(i);
            }
        }
    }

    private bool HasLine(int i)
    {
        for(int j = 0; j < width; j++)
        {
            if(grid[j,i] == null)
                return false;
        }

        return false;
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

    bool ValidMove()
    {
        foreach(Transform children in transform)
        {
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
