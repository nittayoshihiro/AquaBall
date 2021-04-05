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
    /// <summary>スタートキューブプレハブ</summary>
    [SerializeField] GameObject m_startCubePrefab;
    /// <summary>ゴールキューブプレハブ</summary>
    [SerializeField] GameObject m_goalCubePrefab;
    /// <summary>プレイヤープレハブ</summary>
    [SerializeField] GameObject m_playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        CreateMap(Drilling(m_mapSize,1,1), "map",1);
        CreateMap(SetUpMapData(m_mapSize),"floor",0);
    }

    /// <summary>
    /// 穴掘り関数 (0：道　１：壁)
    /// </summary>
    /// <param name="mapsize">マップサイズ</param>
    /// <returns></returns>
    MapState[,] Drilling(int mapsize,int startx,int starty)
    {
        //奇数でないと成り立たないため
        if (m_mapSize % 2 == 1)
        {
            //mapデータを作成　初期化（全て壁）
            MapState[,] mapdata = SetUpMapData(mapsize);
            //穴掘り
            mapdata[startx, starty] = MapState.Start;//スタート
            DigHole(mapdata, startx, starty);
            //ReversDigHole(mapdata, m_goalx, m_goaly);
            return mapdata;
        }
        else
        {
            Debug.LogError("奇数ではないので作成不可");
            return null;
        }
    }

    /// <summary>
    /// マップの初期化
    /// </summary>
    /// <param name="mapsize"></param>
    /// <returns></returns>
    private MapState[,] SetUpMapData(int mapsize)
    {
        MapState[,] mapdata = new MapState[mapsize, mapsize];
        for (int x = 0; x < mapsize; x++)
        {
            for (int y = 0; y < mapsize; y++)
            {
                mapdata[x, y] = MapState.Wall;
            }
        }
        return mapdata;
    }


    /// <summary>
    /// スタートからゴールまで穴を掘る
    /// </summary>
    /// <param name="mapdata">現在のマップデータ</param>
    /// <param name="nowposx">掘り進めている所X</param>
    /// <param name="nowposy">掘り進めている所Y</param>
    private void DigHole(MapState[,] mapdata, int nowposx, int nowposy)
    {
        //上下左右で進める数字を保存する（掘られている所）
        int[] ramDirection = new[] { 0 };
        if (nowposy + 2 < m_mapSize)
        {
            if (mapdata[nowposx, nowposy + 2] == MapState.Wall)//上
            {
                System.Array.Resize(ref ramDirection, ramDirection.Length + 1);
                ramDirection[ramDirection.Length - 1] = 1;
            }
        }
        if (0 < nowposy - 2)
        {
            if (mapdata[nowposx, nowposy - 2] == MapState.Wall)//下
            {
                System.Array.Resize(ref ramDirection, ramDirection.Length + 1);
                ramDirection[ramDirection.Length - 1] = 2;
            }
        }
        if (0 < nowposx - 2)
        {
            if (mapdata[nowposx - 2, nowposy] == MapState.Wall)//左
            {
                System.Array.Resize(ref ramDirection, ramDirection.Length + 1);
                ramDirection[ramDirection.Length - 1] = 3;
            }
        }
        if (nowposx + 2 < m_mapSize)
        {
            if (mapdata[nowposx + 2, nowposy] == MapState.Wall)//右
            {
                System.Array.Resize(ref ramDirection, ramDirection.Length + 1);
                ramDirection[ramDirection.Length - 1] = 4;
            }
        }

        //掘り進められなかったらゴールにする
        if (ramDirection.Length == 1)
        {
            mapdata[nowposx, nowposy] = MapState.Goal;
        }
        else
        {
            int a = ramDirection[Random.Range(1, ramDirection.Length)];
            Debug.Log(a);
            //ランダムで進行方向を決める
            switch (a)
            {
                case 1:
                    mapdata[nowposx, nowposy + 1] = MapState.Road;
                    mapdata[nowposx, nowposy + 2] = MapState.Road;
                    DigHole(mapdata, nowposx, nowposy + 2);
                    break;

                case 2:
                    mapdata[nowposx, nowposy - 1] = MapState.Road;
                    mapdata[nowposx, nowposy - 2] = MapState.Road;
                    DigHole(mapdata, nowposx, nowposy - 2);
                    break;

                case 3:
                    mapdata[nowposx - 1, nowposy] = MapState.Road;
                    mapdata[nowposx - 2, nowposy] = MapState.Road;
                    DigHole(mapdata, nowposx - 2, nowposy);
                    break;

                case 4:
                    mapdata[nowposx + 1, nowposy] = MapState.Road;
                    mapdata[nowposx + 2, nowposy] = MapState.Road;
                    DigHole(mapdata, nowposx + 2, nowposy);
                    break;
            }
        }
    }

    /// <summary>
    /// ゴールから掘り残しを掘る
    /// </summary>
    /// <param name="mapdata">マップデータ</param>
    /// <param name="nowposx">戻り所X</param>
    /// <param name="nowposy">戻り所Y</param>
    private void ReversDigHole(MapState[,] mapdata, int nowposx, int nowposy)
    {
        if (mapdata[nowposx,nowposy]== MapState.Start)
        {
            Debug.Log("終了");
        }
        else
        {
            //上下左右で戻るますを探す
            if (nowposy + 2 < m_mapSize)
            {
                if (mapdata[nowposx, nowposy + 1] == MapState.Road)//上
                {
                    ReversDigHole(mapdata, nowposx, nowposy + 2);
                    DigHole(mapdata, nowposx, nowposy + 2);
                }
            }
            if (0 < nowposy - 2)
            {
                if (mapdata[nowposx, nowposy - 1] == MapState.Road)//下
                {
                    ReversDigHole(mapdata, nowposx, nowposy - 2);
                    DigHole(mapdata, nowposx, nowposy - 2);
                }
            }
            if (0 < nowposx - 2)
            {
                if (mapdata[nowposx - 1, nowposy] == MapState.Road)//左
                {
                    ReversDigHole(mapdata, nowposx - 2, nowposy);
                    DigHole(mapdata, nowposx - 2, nowposy);
                }
            }
            if (nowposx + 2 < m_mapSize)
            {
                if (mapdata[nowposx + 1, nowposy] == MapState.Road)//右
                {
                    ReversDigHole(mapdata, nowposx + 2, nowposy);
                    DigHole(mapdata, nowposx + 2, nowposy);
                }
            }
        }
        
    }


    /// <summary>
    ///　マップのデータを元にマップを作成
    ///　
    /// </summary>
    /// <param name="mapdata">マップデータ</param>
    /// <param name="mapname">親オブジェクトになる名前d</param>
    /// <param name="posy">作る高さ</param>
    void CreateMap(MapState[,] mapdata, string mapname,float posy)
    {
        //親になるオブジェクトを生成
        GameObject mapObject = new GameObject(mapname);
        mapObject.transform.parent = this.gameObject.transform;

        for (int x = 0; x < mapdata.GetLength(0); x++)
        {
            for (int z = 0; z < mapdata.GetLength(1); z++)
            {
                switch (mapdata[x,z])
                {
                    case MapState.Wall:
                        //親の子オブジェクトとして生成
                        Instantiate(m_cubePrefab, new Vector3(x - m_mapSize / 2, posy, z - m_mapSize / 2), Quaternion.identity).gameObject.transform.parent = mapObject.transform;
                        break;
                    case MapState.Start:
                        Instantiate(m_startCubePrefab, new Vector3(x - m_mapSize / 2, posy, z - m_mapSize / 2), Quaternion.identity).gameObject.transform.parent = mapObject.transform;
                        Instantiate(m_playerPrefab, new Vector3(x - m_mapSize / 2, posy, z - m_mapSize / 2), Quaternion.identity);
                        break;
                    case MapState.Goal:
                        Instantiate(m_goalCubePrefab, new Vector3(x - m_mapSize / 2, posy, z - m_mapSize / 2), Quaternion.identity).gameObject.transform.parent = mapObject.transform;
                        break;
                }
            }
        }
    }



    /// <summary>
    /// マップのステータス管理
    /// </summary>
    public enum MapState
    {
        Road,
        Wall,
        Start,
        Goal
    }
}