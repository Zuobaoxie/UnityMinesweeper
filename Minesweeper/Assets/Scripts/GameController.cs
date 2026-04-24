using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    //显示脚本的引用
    private Board board;
    //游戏结束标志位
    //private bool gameover;
    //储存格子数组数据
    private Cell[,] state;

    private void Awake()
    {
        //设置游戏为60帧
        Application.targetFrameRate = 60;
        //获得显示脚本引用
        board = GetComponentInChildren<Board>();
    }
    private void OnEnable()
    {
        EventCenter.AddListener(EventID.WIN, StopTheGame);
        EventCenter.AddListener(EventID.LOSE, StopTheGame);
    }
    private void OnDisable()
    {
        EventCenter.RemoveListener(EventID.WIN, StopTheGame);
        EventCenter.RemoveListener(EventID.LOSE, StopTheGame);
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
        //gameover = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            NewGame();
        }
        else
        {
            if (Input.GetMouseButtonDown(1))//鼠标右键
            {
                Model.Instance.Flag(Input.mousePosition);
            }
            else if (Input.GetMouseButtonDown(0))//这里不写if是因为一帧内只能触发一个语句，如果同时按下鼠标左右键（虽然几乎不可能），这里写elseif就能确保只执行Flag函数
            {
                Model.Instance.Reveal(Input.mousePosition);
            }
        }
      
    }

    private void StopTheGame(object iswin)
    {
        if ((bool)iswin){ Debug.Log("You Win!");}
        else { Debug.Log("You Lose!"); }
        Time.timeScale = 0;
    }


}
