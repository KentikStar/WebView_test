using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTetromino : MonoBehaviour
{
    [SerializeField]
    private GameObject [] tetrominoArr;

    public void IninTetr(){
        int rnd = Random.Range(0, tetrominoArr.Length);

        Instantiate(tetrominoArr[rnd], transform.position, Quaternion.identity);
    }

    // Start is called before the first frame update
    void Start()
    {
        IninTetr();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
