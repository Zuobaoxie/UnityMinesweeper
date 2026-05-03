using UnityEngine;

//--------------------------------【所有内容都属于Data】----------------------------------
public struct CellForProps//这里一开始是struct
{
    //格子类型枚举
    public enum Type
    {
        Invalid,//无效的
        Empty,//空
        Scanner,//扫描仪
    }

    public Vector3Int position;//整形向量，一般的向量是float
    public Type type;
    //public int number;
    public bool revealed;//格子是否已经暴露
    public bool flagged;//是否已经被标记（放入旗子）
    //public bool exploded;//是否爆炸了
}

