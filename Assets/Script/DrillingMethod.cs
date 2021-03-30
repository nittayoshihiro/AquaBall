using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// マップ自動生成、穴掘り法
/// </summary>
public class DrillingMethod : MonoBehaviour
{
    /// <summary>マップのサイズ（奇数）</summary>
    [SerializeField] int m_mapSize;
    /// <summary>キューブプレハブ</summary>
    [SerializeField] GameObject m_cubePrefab;
    /// <summary>スタートとゴールを記録する</summary>
    int startx, starty, goalx, goaly;

    // Start is called before the first frame update
    void Start()
    {
        CreateMap(Drilling(m_mapSize));
    }

    /// <summary>
    /// 穴掘り関数 (0：道　１：壁)
    /// </summary>
    /// <param name="mapsize">マップサイズ</param>
    /// <returns></returns>
    int[,] Drilling(int mapsize)
    {
        //奇数でないと成り立たないため
        if (m_mapSize % 2 == 1)
        {
            //mapデータを作成　初期化（全て壁）
            int[,] mapdata = new int[mapsize, mapsize];
            for (int x = 0; x < mapsize; x++)
            {
                for (int y = 0; y < mapsize; y++)
                {
                    mapdata[x, y] = 1;
                }
            }
            //穴掘り
            mapdata[1, 1] = 0;//スタート
            DigHole(mapdata, 1, 1);
            return mapdata;
        }
        else
        {
            Debug.Log("奇数ではないので作成不可");
            return null;
        }
    }

    /// <summary>
    /// 穴を掘る
    /// </summary>
    /// <param name="mapdata">現在のマップデータ</param>
    /// <param name="nowpos">掘り進めている所</param>
    private void DigHole(int[,] mapdata, int nowposx, int nowposy)
    {
        //上下左右で進める数字を保存する
        int[] ramDirection = new[] { 0 };
        if (nowposy + 2 < m_mapSize)
        {
            if (mapdata[nowposx, nowposy + 2] == 1)//上
            {
                System.Array.Resize(ref ramDirection, ramDirection.Length + 1);
                ramDirection[ramDirection.Length - 1] = 1;
            }
        }
        if (0 <= nowposy - 2)
        {
            if (mapdata[nowposy, nowposy - 2] == 1)//下
            {
                System.Array.Resize(ref ramDirection, ramDirection.Length + 1);
                ramDirection[ramDirection.Length - 1] = 2;
            }
        }
        if (0 <= nowposx - 2)
        {
            if (mapdata[nowposx - 2, nowposy] == 1)//左
            {
                System.Array.Resize(ref ramDirection, ramDirection.Length + 1);
                ramDirection[ramDirection.Length - 1] = 3;
            }
        }
        if (nowposx + 2 < m_mapSize)
        {
            if (mapdata[nowposx + 2, nowposy] == 1)//右
            {
                System.Array.Resize(ref ramDirection, ramDirection.Length + 1);
                ramDirection[ramDirection.Length - 1] = 4;
            }
        }
        if (ramDirection.Length == 1)
        {
            //ゴールを記録する
            goalx = nowposx;
            goaly = nowposy;
        }
        else
        {
            int a = ramDirection[Random.Range(1, ramDirection.Length - 1)];
            Debug.Log(a);
            //ランダムで進行方向を決める
            switch (a)
            {
                case 1:
                    mapdata[nowposx, nowposy + 1] = 0;
                    mapdata[nowposx, nowposy + 2] = 0;
                    DigHole(mapdata, nowposx, nowposy + 2);
                    break;

                case 2:
                    mapdata[nowposx, nowposy - 1] = 0;
                    mapdata[nowposx, nowposy - 2] = 0;
                    DigHole(mapdata, nowposx, nowposy - 2);
                    break;
                case 3:
                    mapdata[nowposx - 1, nowposy] = 0;
                    mapdata[nowposx - 2, nowposy] = 0;
                    DigHole(mapdata, nowposx - 2, nowposy);
                    break;

                case 4:
                    mapdata[nowposx + 1, nowposy] = 0;
                    mapdata[nowposx + 2, nowposy] = 0;
                    DigHole(mapdata, nowposx + 2, nowposy);
                    break;
            }
        }
    }

    /// <summary>
    ///　マップのデータを元にマップを作成
    /// </summary>
    /// <param name="mapdata"></param>
    void CreateMap(int[,] mapdata)
    {
        for (int x = 0; x < mapdata.GetLength(0); x++)
        {
            for (int z = 0; z < mapdata.GetLength(1); z++)
            {
                if (mapdata[x, z] == 1)
                {
                    Instantiate(m_cubePrefab, new Vector3(x - m_mapSize / 2, 0.5f, z - m_mapSize / 2), Quaternion.identity);
                }
            }
        }
    }
}