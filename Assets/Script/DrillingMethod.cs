using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// マップ自動生成、穴掘り法
/// </summary>
public class DrillingMethod : MonoBehaviour
{
    /// <summary>マップのサイズ（奇数）</summary>
    [SerializeField] int m_size;

    // Start is called before the first frame update
    void Start()
    {
        //DrillingMethod();
    }

    /// <summary>
    /// 穴掘り関数
    /// </summary>
    /// <param name="mapsize">マップサイズ</param>
    /// <returns></returns>
    int[,] Drilling(int mapsize)
    {
        int[,] mapdata = new int[mapsize, mapsize];
        //奇数でないと成り立たないため
        if (m_size % 2 == 1)
        {
            return mapdata;
        }
        else
        {
            Debug.Log("奇数ではないので作成不可");
            return null;
        }
    }
}
