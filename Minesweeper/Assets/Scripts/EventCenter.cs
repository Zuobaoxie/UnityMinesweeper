using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class EventCenter : MonoBehaviour
{
    //定义事件表，通过事件ID查询委托的函数，该委托可以接收一个任意输入值（会触发拆箱装箱吗？）
    private static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();
    //订阅事件
    public static void AddListener<T>(string eventId ,Action<T> callback)
    {
        //字典中是否存在这个事件ID
        if (eventTable.ContainsKey(eventId))
        {
            //存在的话就在事件所对应的方法容器里添加方法
            //eventTable[eventId] += callback;//因为Action换成了Delegate所以不能用+=语法糖
            eventTable[eventId] = Delegate.Combine(eventTable[eventId], callback);
        }
        else { eventTable.Add(eventId, callback); }
            
    }
    //移除订阅
    public static void RemoveListener<T>(string eventId, Action<T> callback)
    {
        if (eventTable.ContainsKey(eventId))
        {
            //eventTable[eventId] -= callback;
            eventTable[eventId] = Delegate.Remove(eventTable[eventId], callback);
        }
    }
    //分发事件（可以携带数据）
    public static void DisPatch<T>(string eventId,T data)
    {
        //本来的写法：
        //if (eventTable.ContainsKey(eventId))
        //{
        //    //断帧
        //    //eventTable[eventId]?.Invoke(data);
        //    Action<T> action = eventTable[eventId] as Action<T>;
        //    action?.Invoke(data);
        //}
        //性能更好更安全的写法
        if (eventTable.TryGetValue(eventId, out var del) && del is Action<T> action)
        {
            //TryGetValue:尝试从字典中获取事件ID对应的委托。 找到了该事件，将委托赋值给 del;没找到，del 为 null
            //del is Action<T> action:检查 del 是否可以转换为 Action<T>,如果可以，转换并赋值给 action
            action?.Invoke(data);
        }
    }
}
