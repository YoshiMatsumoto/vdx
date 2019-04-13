using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


public class VDX_Editor
{

    /// -------------------------------------------
    /// 2018/10/06 Matsumoto
    /// リストをシャッフルする関数
    /// -------------------------------------------
    public static void Shuffle<T>(List<T> array, int Seed)
    {
        System.Random r = new System.Random(Seed);
        int n = array.Count;
        while (n > 1)
        {
            int k = r.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }

    /// -------------------------------------------
    /// 2019/03/05 Matsumoto
    /// リストをデバッグログへ出力する関数
    /// -------------------------------------------
    public string ShowListContentsInTheDebug<T>(List<T> list)
    {
        string log = "";
        foreach (var content in list.Select((val, idx) => new { val, idx }))
        {
            if (content.idx == list.Count - 1)
                log += content.val.ToString();
            else
                log += content.val.ToString() + ", ";
        }
        return log;
    }
}
