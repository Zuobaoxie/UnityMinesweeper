using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopEventData
{
    public bool isWin;//获胜标志位
    public int mineSum;//显示地雷数

    //构造函数
    public StopEventData(bool isWin, int mineSum)
    {
        this.isWin = isWin;
        this.mineSum = mineSum;
    }
}