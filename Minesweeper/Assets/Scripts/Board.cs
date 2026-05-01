using UnityEngine;
using UnityEngine.Tilemaps;//Tilemap的命名空间

//声明该脚本依赖于Tilemap组件
[RequireComponent(typeof(Tilemap))]
//--------------------------------【所有内容都属于View】----------------------------------
public class Board : MonoBehaviour
{
    //获得Tilemap引用
    public Tilemap tilemap { get; private set; }//后续的脚本需要访问并读取贴图，因此get是公有的

    public Tile tileUnknown;
    public Tile tileEmpty; 
    public Tile tileMine;
    public Tile tileExploded;
    public Tile tileFlag;
    public Tile tileNum1;
    public Tile tileNum2;
    public Tile tileNum3;
    public Tile tileNum4;
    public Tile tileNum5;
    public Tile tileNum6;
    public Tile tileNum7;
    public Tile tileNum8;

    private void Awake()
    {
        //获得Tilemap组件
        tilemap = GetComponent<Tilemap>();
    }

    private void Start()
    {
        //调整相机位置使版面总是位于中心
        Camera.main.transform.position = new Vector3(Model.Instance.width / 2f, Model.Instance.height / 2f, -10f);
    }

    //读取二维数组中每个格子的状态并显示
    public void Draw(Cell[,] state)//二维Cell数组可以理解为int[,]
    {
        //这个state就是来存储每个格子是什么状态的数据的
        int width = state.GetLength(0);//返回行数
        int height = state.GetLength(1);//返回列数

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];//读取坐标（索引）为（x,y）的格子的所有数据
                //让tilemap显示  位置参数        要放哪个图，1，2还是empty
                tilemap.SetTile(cell.position, GetTile(cell));
            }
        }
    }


    private Tile GetTile(Cell cell)
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
            return tileFlag;
        } 
        else //还未进行交互
        {
            return tileUnknown;
        }
    }


    //确定需要显示哪种格子
    private Tile GetRevealedTile(Cell cell)
    {
        //检查格子类型
        switch (cell.type)
        {
            case Cell.Type.Empty: return tileEmpty;
            //如果已经爆炸，就返回爆炸的地雷贴图，没有就返回地雷
            case Cell.Type.Mine: return cell.exploded ? tileExploded : tileMine;
            case Cell.Type.Number: return GetNumberTile(cell);
            default: return null;
        }
    }

    //确定需要显示哪个数字
    private Tile GetNumberTile(Cell cell)
    {
        switch (cell.number)
        {
            case 1: return tileNum1;
            case 2: return tileNum2;
            case 3: return tileNum3;
            case 4: return tileNum4;
            case 5: return tileNum5;
            case 6: return tileNum6;
            case 7: return tileNum7;
            case 8: return tileNum8;
            default: return null;
        }
    }

}
