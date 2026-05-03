using UnityEngine;
using UnityEngine.Tilemaps;//Tilemap的命名空间

//声明该脚本依赖于Tilemap组件
[RequireComponent(typeof(Tilemap))]
//--------------------------------【所有内容都属于View】----------------------------------
public class BoardForProps : MonoBehaviour
{
    //获得Tilemap引用
    public Tilemap tilemap { get; private set; }//后续的脚本需要访问并读取贴图，因此get是公有的
    public Tile tilescanner;
    public Tile propsEmpty; 
    

    private void Awake()
    {
        //获得Tilemap组件
        tilemap = GetComponent<Tilemap>();
    }

    //读取二维数组中每个格子的状态并显示
    public void DrawProps(CellForProps[,] state)//二维Cell数组可以理解为int[,]
    {
        //这个state就是来存储每个格子是什么状态的数据的
        int width = state.GetLength(0);//返回行数
        int height = state.GetLength(1);//返回列数

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                CellForProps cell = state[x, y];//读取坐标（索引）为（x,y）的格子的所有数据
                //让tilemap显示  位置参数        要放哪个图，1，2还是empty
                tilemap.SetTile(cell.position, GetTile(cell));
            }
        }
    }


    private Tile GetTile(CellForProps cell)
    {
        //已经暴露出来
        if (cell.revealed) 
        {
            //确定需要显示哪种格子
            return GetRevealedTile(cell);
        } 
        //已经被标记
        else if (cell.flagged) 
        {
            return propsEmpty;
        } 
        else //还未进行交互
        {
            return propsEmpty;
        }
    }


    //确定需要显示哪种格子
    private Tile GetRevealedTile(CellForProps cell)
    {
        //检查格子类型
        switch (cell.type)
        {
            case CellForProps.Type.Empty: return propsEmpty;
            //如果已经爆炸，就返回爆炸的地雷贴图，没有就返回地雷
            case CellForProps.Type.Scanner: return tilescanner;
            default: return null;
        }
    }


}
