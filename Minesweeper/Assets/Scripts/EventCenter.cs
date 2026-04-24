using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//定义事件，以后增加事件只需要在这里修改
public class EventID
{
    public const string WIN = "Win";
    public const string LOSE = "Lose";
}
public class EventCenter : MonoBehaviour
{
    //定义事件表，通过事件ID查询委托的函数，该委托可以接收一个任意输入值（会触发拆箱装箱吗？）
    private static Dictionary<string, Action<object>> eventTable = new Dictionary<string, Action<object>>();
    //订阅事件
    public static void AddListener(string eventId ,Action<object> callback)
    {
        //字典中是否存在这个事件ID
        if (eventTable.ContainsKey(eventId))
        {
            //存在的话就在事件所对应的方法容器里添加方法
            eventTable[eventId] += callback;
        }
        else { eventTable.Add(eventId, callback); }
            
    }
    //移除订阅
    public static void RemoveListener(string eventId, Action<object> callback)
    {
        if (eventTable.ContainsKey(eventId))
        {
            eventTable[eventId] -= callback;
        }
    }
    //分发事件（可以携带数据）
    public static void DisPatch(string eventId,object data = null)
    {
        if (eventTable.ContainsKey(eventId))
        {
            eventTable[eventId]?.Invoke(data);
        }
    }
}
