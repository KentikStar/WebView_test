using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTetromino : MonoBehaviour
{
    [SerializeField]
    private GameObject [] tetrominoArr;

    public void IninTetr(){
        int rnd = Random.Range(0, tetrominoArr.Length);

        Instantiate(tetrominoArr[rnd], transform.position, Quaternion.identity, transform);
    }

    public void ClearChild(){
        foreach(Transform children in transform)
        {
            Destroy(children.gameObject);
        }
    }
}
