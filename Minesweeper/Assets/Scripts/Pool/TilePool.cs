using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//这个是把对象池和对象池管理器做成了一块
public class TilePool : MonoBehaviour
{
    //显示：Tilemap.setTile(位置,tile)
    //传统的Tilemap做法是获得这个tile引用,然后将tile显示;不用的时候将tile引用从内部字典中删除
    //添加了对象池的做法是先使用ScriptableObjectl创建实例tile预存到对象池中,通过setTile显示;不用的时候回收到对象池
    //单例
    private static TilePool instance;
    public static TilePool Instance => instance;
    //字典，通过Tile对象查找对象池
    private Dictionary<Tile, Queue<Tile>> pools = new Dictionary<Tile, Queue<Tile>>();

    private void Awake()
    {
        instance = this;//单例常规操作
    }

    //添加对象池
    public void RegisterPool(Tile original, int prewarmCount = 10)
    {
        //对象池管理器里已经有该对象的对象池
        if (pools.ContainsKey(original)) return;
        //往对象池管理器里添加新对象池
        pools[original] = new Queue<Tile>();
        //预先准备好对象
        for (int i = 0; i < prewarmCount; i++)
        {
            //生成实例
            Tile newTile = CreateTileCopy(original);
            //对象入队
            pools[original].Enqueue(newTile);
        }
    }
    //创建Tile
    private Tile CreateTileCopy(Tile original)
    {
        //tile实例化
        //Tile类本身是Unity内置的ScriptableObject类型，因此想要实例化Tile则需要调用ScriptableObject类方法
        Tile copy = ScriptableObject.CreateInstance<Tile>();
        // 复制Tile的所有属性
        copy.sprite = original.sprite;
        copy.color = original.color;
        copy.colliderType = original.colliderType;
        copy.flags = original.flags;
        return copy;
    }
    //获得对象
    public Tile GetTile(Tile original)
    {
        if (!pools.ContainsKey(original))
        {
            RegisterPool(original);
        }

        if (pools[original].Count > 0)
        {
            return pools[original].Dequeue();
        }

        return CreateTileCopy(original);
    }
    //回收对象
    public void ReturnTile(Tile tile, Tile original)
    {
        if (pools.ContainsKey(original))
        {
            pools[original].Enqueue(tile);
        }
    }
}
