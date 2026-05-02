using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameController : MonoBehaviour
{

    //显示脚本的引用
    private Board board;
    //游戏结束标志位
    //private bool gameover;
    //储存格子数组数据
    //private Cell[,] state;
    //定义事件,描述数据生成完成
    //public event Action<Cell[,]> OnCellsDataGenerated;

    private void Awake()
    {
        //设置游戏为60帧
        Application.targetFrameRate = 60;
        //获得显示脚本引用
        board = GetComponentInChildren<Board>();
    }
    private void OnEnable()
    {
        EventCenter.AddListener<StopEventData>(EventID.WIN, StopTheGame);
        EventCenter.AddListener<StopEventData>(EventID.LOSE, StopTheGame);
    }
    private void OnDisable()
    {
        EventCenter.RemoveListener<StopEventData>(EventID.WIN, StopTheGame);
        EventCenter.RemoveListener<StopEventData>(EventID.LOSE, StopTheGame);
    }

    private void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
        // 订阅完成事件
        //Model.Instance.OnCellsDataGenerated += OnDataGenerated;
        EventCenter.AddListener<Cell[,]>(EventID.DataGenerated, OnDataGenerated);
        // 发送通知让Model干活
        Model.Instance.GenerateCellsDataAsync();        
    }

    private void OnDataGenerated(Cell[,] state)
    {
        // 取消订阅，避免重复调用
        //Model.Instance.OnCellsDataGenerated -= OnDataGenerated;
        EventCenter.RemoveListener<Cell[,]>(EventID.DataGenerated, OnDataGenerated);
        //调用面板脚本根据二维数组将格子在游戏上显示
        board.Draw(state);
        board.SetBoardToCenter();
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

    private void StopTheGame(StopEventData data)
    {
        if (data.isWin){ Debug.Log("You Win! " + " Mine:" + data.mineSum);}
        else { Debug.Log("You Lose! "+ " Mine:" + data.mineSum); }
        Time.timeScale = 0;
        //Debug.Log("游戏暂停");
    }


}
