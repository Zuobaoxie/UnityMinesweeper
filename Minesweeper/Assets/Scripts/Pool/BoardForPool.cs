using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;//Tilemap的命名空间

//声明该脚本依赖于Tilemap组件
[RequireComponent(typeof(Tilemap))]
//--------------------------------【所有内容都属于View】----------------------------------
public class BoardForPool : MonoBehaviour
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

    // 用于存储当前显示的Tile实例（用于归还池）
    private Dictionary<Vector3Int, Tile> currentTiles = new Dictionary<Vector3Int, Tile>();
    private void Awake()
    {
        //获得Tilemap组件
        tilemap = GetComponent<Tilemap>();
    }
    private void Start()
    {
        //初始化Tile池
        InitializeTilePools();
    }
    private void InitializeTilePools()
    {
        // 准备Tile
        TilePool.Instance.RegisterPool(tileUnknown, 50);
        TilePool.Instance.RegisterPool(tileEmpty, 100);
        TilePool.Instance.RegisterPool(tileMine, 30);
        TilePool.Instance.RegisterPool(tileExploded, 10);
        TilePool.Instance.RegisterPool(tileFlag, 20);
        TilePool.Instance.RegisterPool(tileNum1, 10);
        TilePool.Instance.RegisterPool(tileNum2, 10);
        TilePool.Instance.RegisterPool(tileNum3, 10);
        TilePool.Instance.RegisterPool(tileNum4, 10);
        TilePool.Instance.RegisterPool(tileNum5, 10);
        TilePool.Instance.RegisterPool(tileNum6, 10);
        TilePool.Instance.RegisterPool(tileNum7, 10);
        TilePool.Instance.RegisterPool(tileNum8, 10);
    }

    //读取二维数组中每个格子的状态并显示
    public void Draw(Cell[,] state)//二维Cell数组可以理解为int[,]
    {
        //这个state就是来存储每个格子是什么状态的数据的
        int width = state.GetLength(0);//返回行数
        int height = state.GetLength(1);//返回列数

        //用哈希表记录所有需要清理的位置（之前显示过但现在可能不用的格子）
        HashSet<Vector3Int> toClear = new HashSet<Vector3Int>(currentTiles.Keys);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];//读取坐标（索引）为（x,y）的格子的所有数据
                //让tilemap显示  位置参数        要放哪个图，1，2还是empty
                Vector3Int pos = cell.position;
                //获取需要的Tile类型（从池中取）
                Tile neededTile = GetTileFromPool(cell);
                //获取当前这个位置显示的Tile
                Tile currentTile = tilemap.GetTile<Tile>(pos);
                //如果需要显示的Tile和当前显示的不同
                if (currentTile != neededTile)
                {
                    //将旧的Tile归还到池中
                    if (currentTile != null)
                    {
                        ReturnTileToPool(currentTile, GetOriginalTileForType(currentTile));
                    }
                    //设置新的Tile
                    tilemap.SetTile(pos, neededTile);
                    //记录这个位置现在显示的Tile
                    currentTiles[pos] = neededTile;
                }
                //这个位置仍然在使用，不需要清除
                toClear.Remove(pos);

            }
        }

        // 清除不再需要的格子
        foreach (Vector3Int pos in toClear)
        {
            Tile currentTile = tilemap.GetTile<Tile>(pos);
            if (currentTile != null)
            {
                ReturnTileToPool(currentTile, GetOriginalTileForType(currentTile));
            }
            tilemap.SetTile(pos, null);
            currentTiles.Remove(pos);
        }
    }

    //从池中获取对象
    private Tile GetTileFromPool(Cell cell)
    {
        Tile originalTile = GetTile(cell);
        if (originalTile == null) return null;

        return TilePool.Instance.GetTile(originalTile);
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
            default: return tileUnknown;
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
    //根据Tile实例找到对应的原始Tile
    private Tile GetOriginalTileForType(Tile tileInstance)
    {
        // 通过Sprite来判断是哪种Tile（简单方案）
        if (tileInstance.sprite == tileUnknown.sprite) return tileUnknown;
        if (tileInstance.sprite == tileEmpty.sprite) return tileEmpty;
        if (tileInstance.sprite == tileMine.sprite) return tileMine;
        if (tileInstance.sprite == tileExploded.sprite) return tileExploded;
        if (tileInstance.sprite == tileFlag.sprite) return tileFlag;
        if (tileInstance.sprite == tileNum1.sprite) return tileNum1;
        if (tileInstance.sprite == tileNum2.sprite) return tileNum2;
        if (tileInstance.sprite == tileNum3.sprite) return tileNum3;
        if (tileInstance.sprite == tileNum4.sprite) return tileNum4;
        if (tileInstance.sprite == tileNum5.sprite) return tileNum5;
        if (tileInstance.sprite == tileNum6.sprite) return tileNum6;
        if (tileInstance.sprite == tileNum7.sprite) return tileNum7;
        if (tileInstance.sprite == tileNum8.sprite) return tileNum8;

        return null;
    }

    private void ReturnTileToPool(Tile tile, Tile original)
    {
        if (tile != null && original != null)
        {
            TilePool.Instance.ReturnTile(tile, original);
        }
    }

    public void ClearBoard()
    {
        foreach (var kvp in currentTiles)
        {
            Tile original = GetOriginalTileForType(kvp.Value);
            if (original != null)
            {
                TilePool.Instance.ReturnTile(kvp.Value, original);
            }
        }

        tilemap.ClearAllTiles();
        currentTiles.Clear();
    }

    private void OnDestroy()
    {
        //组件销毁时归还所有Tile
        ClearBoard();
    }

}
