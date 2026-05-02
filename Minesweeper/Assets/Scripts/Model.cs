using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Model : MonoBehaviour
{
    private static Model instance;
    public static Model Instance
    {
        get => instance;
        set => instance = value;
    }
    private void Awake()
    {
        instance = this;
    }

    public int width = 16;//地图宽度
    public int height = 16;//地图长度
    public int mineCount = 32;
    //储存格子数组数据
    private Cell[,] state;
    public Board board;
    //private bool gameOver;
    //定义事件,描述数据生成完成
    //public event Action<Cell[,]> OnCellsDataGenerated;

    //Unity自带的函数，面板值被改变时调用
    private void OnValidate()
    {
        //确保雷的数量在某个范围内
        mineCount = Mathf.Clamp(mineCount, 0, width * height);
    }

    // 异步生成数据
    public void GenerateCellsDataAsync()
    {
        // 使用协程异步执行
        StartCoroutine(GenerateCellsDataCoroutine());
    }

    //协程
    private IEnumerator GenerateCellsDataCoroutine()
    {
        state = new Cell[width, height];
        //这个函数生成数据，并把数据存到二维数组
        GenerateCells();
        yield return null;//等待一帧
        GenerateMines();
        yield return null;//等待一帧
        GenerateNumbers();
        yield return null;//等待一帧
        // 数据生成完成，触发事件回调，回调时传入state参数。
        EventCenter.DisPatch(EventID.DataGenerated, state);
        //OnCellsDataGenerated?.Invoke(state);
    }

    //public Cell[,] GenerateCellsData()
    //{
    //    state = new Cell[width, height];
    //    //这个函数生成数据，并把数据存到二维数组
    //    GenerateCells();
    //    GenerateMines();
    //    GenerateNumbers();
    //    return state;
    //}


    //生成普通格子
    private void GenerateCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = new Cell();
                cell.position = new Vector3Int(x, y, 0);
                //初始的时候都是空的
                cell.type = Cell.Type.Empty;
                //存到二维数组中
                state[x, y] = cell;
            }
        }
    }

    //生成地雷
    public void GenerateMines()
    {
        for (int i = 1; i <= mineCount; i++)//i要从1开始否则生成的地雷会比mineCount多1
        {
            //随机产生作坐标，供后续生成地雷
            int x = UnityEngine.Random.Range(0, width);
            int y = UnityEngine.Random.Range(0, height);
            //考虑这个坐标已经有地雷的情况
            while (state[x, y].type == Cell.Type.Mine)
            {
                x++;

                if (x >= width)//考虑边缘情况
                {
                    x = 0;
                    y++;

                    if (y >= height)
                    {
                        y = 0;
                    }
                }

            }
            state[x, y].type = Cell.Type.Mine;
        }
    }

    //生成数字格子
    public void GenerateNumbers()
    {

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];

                if (cell.type == Cell.Type.Mine)
                {
                    continue;
                }
                cell.number = CountMines(x, y);//计算格子周围地雷数
                if (cell.number > 0)
                {
                    cell.type = Cell.Type.Number;
                }
                //将修改存到数组中
                state[x, y] = cell;
            }
        }
    }

    //计算周围有多少雷
    private int CountMines(int CellX, int CellY)
    {
        int count = 0;
        //遍历这个格子周围的八个格子
        for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
        {
            for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
            {
                if (adjacentX == 0 && adjacentY == 0)//格子本身
                {
                    continue;
                }

                int x = CellX + adjacentX;
                int y = CellY + adjacentY;

                if (GetCell(x, y).type == Cell.Type.Mine)
                {
                    count++;
                }
            }
        }

        return count;
    }

    public void Flag(Vector3 mousePosition)
    {
        //将屏幕坐标转换为世界坐标,相机类功能
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        //将世界坐标转换为贴图地图坐标，Tilemap功能
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);//好神奇贴图可以转换3D坐标
        Cell cell = GetCell(cellPosition.x, cellPosition.y);
        //Debug.Log(cellPosition.x + "," + cellPosition.y);
        //如果被点击的是面板以外的区域或者这个格子已经被暴露出来
        if (cell.type == Cell.Type.Invalid || cell.revealed) { return; }
        cell.flagged = !cell.flagged;
        //更新数据
        state[cellPosition.x, cellPosition.y] = cell;
        //更新面板
        board.Draw(state);

    }

    //通过鼠标点击的坐标获取到格子
    private Cell GetCell(int x, int y)
    {
        //判断格子是否有效
        if (isValid(x, y))
        {
            return state[x, y];
        }
        else { return new Cell(); }
    }

    //判断鼠标是否点击到了格子区
    private bool isValid(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public bool Reveal(Vector3 mousePosition)
    {
        //将屏幕坐标转换为世界坐标,相机类功能
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        //将世界坐标转换为贴图地图坐标，Tilemap功能
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);//好神奇贴图可以转换3D坐标
        Cell cell = GetCell(cellPosition.x, cellPosition.y);
        bool isGameOver = false;
        if (cell.type == Cell.Type.Invalid || cell.revealed || cell.flagged) { return false; }
        switch (cell.type)
        {
            case Cell.Type.Mine:
                isGameOver = Explode(cell);
                break;
            case Cell.Type.Empty:
                Flood(cell);
                CheckWinCondition();
                break;
            default:
                cell.revealed = true;
                //更新数据
                state[cellPosition.x, cellPosition.y] = cell;
                CheckWinCondition();
                break;
        }
        //更新面板
        board.Draw(state);
        return isGameOver;
    }
    //爆炸
    private bool Explode(Cell cell)
    {        
        //先修改格子状态
        cell.exploded = true;
        cell.revealed = true;
        //然后更新存储的数据
        state[cell.position.x, cell.position.y] = cell;
        //显示其他所有地雷
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cell = state[x, y];
                if (cell.type == Cell.Type.Mine)
                {
                    cell.revealed = true;
                    state[x, y] = cell;
                }
            }
        }
        //广播玩家输了
        var data = new StopEventData(false, mineCount);
        EventCenter.DisPatch(EventID.LOSE,data);
        return true;
    }

    //洪范递归
    private void Flood(Cell cell)
    {
        //已经被揭露
        if (cell.revealed) { return; }
        if (cell.type == Cell.Type.Mine || cell.type == Cell.Type.Invalid) { return; }
        //这里也考虑了数字的情况，如果是数字就需要被揭露，但不应该继续泛滥下去，所以这样写。
        cell.revealed = true;
        //更新数据
        state[cell.position.x, cell.position.y] = cell;
        if (cell.type == Cell.Type.Empty)
        {
            Flood(GetCell(cell.position.x + 1, cell.position.y));
            Flood(GetCell(cell.position.x - 1, cell.position.y));
            Flood(GetCell(cell.position.x, cell.position.y + 1));
            Flood(GetCell(cell.position.x, cell.position.y - 1));
        }

    }

    private void CheckWinCondition()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];

                //所有不是雷的格子都暴露了才算赢
                //所以这里判断是否有不是雷的格子还没暴露，有的话就继续游戏
                if (cell.type != Cell.Type.Mine && !cell.revealed)
                {
                    return; // no win
                }
            }
        }

        //这里是为了在赢了之后用旗子显示地雷
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];
                if (cell.type == Cell.Type.Mine)
                {
                    cell.flagged = true;
                    state[x, y] = cell;
                }
            }
        }
        //广播赢
        var data = new StopEventData(true, mineCount);
        EventCenter.DisPatch(EventID.WIN,data);
    }
}
