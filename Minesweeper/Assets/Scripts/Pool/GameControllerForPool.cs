using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerForPool : MonoBehaviour
{

    //显示脚本的引用
    private BoardForPool board;
    //游戏结束标志位
    private bool gameover;
    //储存格子数组数据
    private Cell[,] state;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        //获得显示脚本引用
        board = GetComponentInChildren<BoardForPool>();
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
        state = ModelForPool.Instance.GenerateCellsData();
        //------------Model--------------
        //调用面板脚本根据二维数组将格子在游戏上显示
        board.Draw(state);
        //调整相机位置使版面总是位于中心
        Camera.main.transform.position = new Vector3(ModelForPool.Instance.width / 2f, ModelForPool.Instance.height / 2f, -10f);
        gameover = false;
    }

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
                ModelForPool.Instance.Flag(Input.mousePosition);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                gameover = ModelForPool.Instance.Reveal(Input.mousePosition);
            }
        }

    }
    private void StopTheGame(object iswin)
    {
        if ((bool)iswin) { Debug.Log("You Win!"); }
        else { Debug.Log("You Lose!"); }
        Time.timeScale = 0;
    }

}
