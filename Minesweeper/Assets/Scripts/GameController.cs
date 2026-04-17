using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //--------------Model-------------

    //-------------Model--------------

    //显示脚本的引用
    private Board board;
    //游戏结束标志位
    private bool gameover;
    //储存格子数组数据
    private Cell[,] state;


    //------------------M-------------------

    //------------------M-------------------

    private void Awake()
    {
        Application.targetFrameRate = 60;
        //获得显示脚本引用
        board = GetComponentInChildren<Board>();
    }

    private void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
        //-------------Model--------------
        state = Model.Instance.GenerateCellsData();
        //------------Model--------------
        //调用面板脚本根据二维数组将格子在游戏上显示
        board.Draw(state);
        //调整相机位置使版面总是位于中心
        Camera.main.transform.position = new Vector3(Model.Instance.width / 2f, Model.Instance.height / 2f, -10f);
        gameover = false;
    }

    //------------Model--------------
    

    //------------Model--------------

    //----------------------------------Controller内容
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            NewGame();
        }
        else if (!gameover)
        {
            if (Input.GetMouseButtonDown(1))//鼠标右键
            {
                Model.Instance.Flag(Input.mousePosition);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                gameover = Model.Instance.Reveal(Input.mousePosition);
            }
        }

    }

    //------------Controller---------
    //------------Model--------------
    //------------Model--------------
}
