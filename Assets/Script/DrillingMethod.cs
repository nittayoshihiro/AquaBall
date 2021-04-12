using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// マップ自動生成、穴掘り法
/// </summary>
public class DrillingMethod : MonoBehaviour
{
    /// <summary>マップの名前</summary>
    [SerializeField] string m_mapName = "map";
    /// <summary>マップのサイズ（奇数）</summary>
    [SerializeField] int m_mapSize;
    /// <summary>作成する階層</summary>
    [SerializeField] int m_mapFloor;
    /// <summary>キューブプレハブ</summary>
    [SerializeField] GameObject m_cubePrefab;
    /// <summary>スタートキューブプレハブ</summary>
    [SerializeField] GameObject m_startCubePrefab;
    /// <summary>中間地点キューブプレハブ</summary>
    [SerializeField] GameObject m_middleCubePrefab;
    /// <summary>ゴールキューブプレハブ</summary>
    [SerializeField] GameObject m_goalCubePrefab;
    /// <summary>プレイヤープレハブ</summary>
    [SerializeField] GameObject m_playerPrefab;
    /// <summary>デバッグonoff</summary>
    [SerializeField] bool m_debug = true;
    //[SerializeField] int m_startx, m_startz;
    int m_goalx, m_goalz;

    public void TestMapping()
    {
        //子オブジェクトを削除（マップ）
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }
        GameObject player = GameObject.Find("Player(Clone)");
        Destroy(player);
        CreateMap(m_mapName, m_mapFloor);
    }

    // Start is called before the first frame update
    public void MappingStart()
    {
        CreateMap(m_mapName, m_mapFloor);
    }

    /// <summary>
    /// マップを作成
    /// </summary>
    ///<param name="mapname">マップの名前</param>
    ///<param name="mapfloor">作る階層</param>
    public void CreateMap(string mapname, int mapfloor)
    {
        CreateFloorMap(Drilling(m_mapSize, 1, 1), SetUpMapData(m_mapSize), mapname, mapfloor);
    }

    /// <summary>
    /// 穴掘り関数 (0：道　１：壁)
    /// </summary>
    /// <param name="mapsize">マップサイズ</param>
    /// <param name="startx">スタートポジションx</param>
    /// <param name="starty">スタートポジションy</param>
    MapState[,] Drilling(int mapsize, int startx, int starty)
    {
        //奇数でないと成り立たないため
        if (m_mapSize % 2 == 1)
        {
            //mapデータを作成　初期化（全て壁）
            MapState[,] mapdata = SetUpMapData(mapsize);
            //穴掘り
            mapdata[startx, starty] = MapState.Start;//スタート
            DigHole(mapdata, startx, starty);
            //ReversDigHole(mapdata, m_goalx, m_goalz);
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
            for (int z = 0; z < mapsize; z++)
            {
                mapdata[x, z] = MapState.Wall;
            }
        }
        return mapdata;
    }


    /// <summary>
    /// スタートからゴールまで穴を掘る
    /// </summary>
    /// <param name="mapdata">現在のマップデータ</param>
    /// <param name="nowposx">掘り進めている所X</param>
    /// <param name="nowposz">掘り進めている所Y</param>
    private void DigHole(MapState[,] mapdata, int nowposx, int nowposz)
    {
        //上下左右で進める数字を保存する（掘られている所）
        int[] ramDirection = new[] { 0 };
        if (nowposz + 2 < m_mapSize)
        {
            if (mapdata[nowposx, nowposz + 2] == MapState.Wall)//上
            {
                System.Array.Resize(ref ramDirection, ramDirection.Length + 1);
                ramDirection[ramDirection.Length - 1] = 1;
            }
        }
        if (0 < nowposz - 2)
        {
            if (mapdata[nowposx, nowposz - 2] == MapState.Wall)//下
            {
                System.Array.Resize(ref ramDirection, ramDirection.Length + 1);
                ramDirection[ramDirection.Length - 1] = 2;
            }
        }
        if (0 < nowposx - 2)
        {
            if (mapdata[nowposx - 2, nowposz] == MapState.Wall)//左
            {
                System.Array.Resize(ref ramDirection, ramDirection.Length + 1);
                ramDirection[ramDirection.Length - 1] = 3;
            }
        }
        if (nowposx + 2 < m_mapSize)
        {
            if (mapdata[nowposx + 2, nowposz] == MapState.Wall)//右
            {
                System.Array.Resize(ref ramDirection, ramDirection.Length + 1);
                ramDirection[ramDirection.Length - 1] = 4;
            }
        }

        //掘り進められなかったらゴールにする
        if (ramDirection.Length == 1)
        {
            mapdata[nowposx, nowposz] = MapState.Goal;
        }
        else
        {
            //デバッグでするため変数に入れる
            int ramTestDirection = ramDirection[Random.Range(1, ramDirection.Length)];
            if (m_debug)
            {
                Debug.Log(ramTestDirection);
            }
            //ランダムで進行方向を決める
            switch (ramTestDirection)
            {
                case 1:
                    mapdata[nowposx, nowposz + 1] = MapState.Road;
                    mapdata[nowposx, nowposz + 2] = MapState.Road;
                    DigHole(mapdata, nowposx, nowposz + 2);
                    break;

                case 2:
                    mapdata[nowposx, nowposz - 1] = MapState.Road;
                    mapdata[nowposx, nowposz - 2] = MapState.Road;
                    DigHole(mapdata, nowposx, nowposz - 2);
                    break;

                case 3:
                    mapdata[nowposx - 1, nowposz] = MapState.Road;
                    mapdata[nowposx - 2, nowposz] = MapState.Road;
                    DigHole(mapdata, nowposx - 2, nowposz);
                    break;

                case 4:
                    mapdata[nowposx + 1, nowposz] = MapState.Road;
                    mapdata[nowposx + 2, nowposz] = MapState.Road;
                    DigHole(mapdata, nowposx + 2, nowposz);
                    break;
            }
        }
    }

    /// <summary>
    /// ゴールから掘り残しを掘る
    /// </summary>
    /// <param name="mapdata">マップデータ</param>
    /// <param name="nowposx">戻り所X</param>
    /// <param name="nowposz">戻り所Y</param>
    private void ReversDigHole(MapState[,] mapdata, int nowposx, int nowposz)
    {
        if (mapdata[nowposx, nowposz] == MapState.Start)
        {
            Debug.Log("終了");
        }
        else
        {
            //上下左右で戻るますを探す
            if (nowposz + 2 < m_mapSize)
            {
                if (mapdata[nowposx, nowposz + 1] == MapState.Road)//上
                {
                    ReversDigHole(mapdata, nowposx, nowposz + 2);
                    DigHole(mapdata, nowposx, nowposz + 2);
                }
            }
            if (0 < nowposz - 2)
            {
                if (mapdata[nowposx, nowposz - 1] == MapState.Road)//下
                {
                    ReversDigHole(mapdata, nowposx, nowposz - 2);
                    DigHole(mapdata, nowposx, nowposz - 2);
                }
            }
            if (0 < nowposx - 2)
            {
                if (mapdata[nowposx - 1, nowposz] == MapState.Road)//左
                {
                    ReversDigHole(mapdata, nowposx - 2, nowposz);
                    DigHole(mapdata, nowposx - 2, nowposz);
                }
            }
            if (nowposx + 2 < m_mapSize)
            {
                if (mapdata[nowposx + 1, nowposz] == MapState.Road)//右
                {
                    ReversDigHole(mapdata, nowposx + 2, nowposz);
                    DigHole(mapdata, nowposx + 2, nowposz);
                }
            }
        }

    }


    /// <summary>
    ///　マップのデータを元にフロアマップを作成
    /// </summary>
    /// <param name="mapdata">マップデータ</param>
    /// <param name="mapname">親オブジェクトになる名前d</param>
    /// <param name="maxfloor">作る高さ(maxの高さ)</param>
    void CreateFloorMap(MapState[,] mapdata, MapState[,] floormapdata, string mapname, float maxfloor)
    {
        //親になるオブジェクトを生成
        GameObject mapObject = new GameObject(mapname);
        mapObject.transform.parent = this.gameObject.transform;

        for (int x = 0; x < mapdata.GetLength(0); x++)
        {
            for (int z = 0; z < mapdata.GetLength(1); z++)
            {
                //親の子オブジェクトとして生成
                switch (mapdata[x, z])
                {
                    case MapState.Wall:
                        Instantiate(m_cubePrefab, new Vector3(x - m_mapSize / 2, maxfloor, z - m_mapSize / 2), Quaternion.identity).gameObject.transform.parent = mapObject.transform;
                        break;
                    case MapState.Start:
                        //最初だけ作成※マジックナンバー
                        if (0 == maxfloor)
                        {
                            Instantiate(m_startCubePrefab, new Vector3(x - m_mapSize / 2, maxfloor, z - m_mapSize / 2), Quaternion.identity).gameObject.transform.parent = mapObject.transform;
                            Instantiate(m_playerPrefab, new Vector3(x - m_mapSize / 2, maxfloor, z - m_mapSize / 2), Quaternion.identity);
                        }
                        else
                        {
                            Instantiate(m_middleCubePrefab, new Vector3(x - m_mapSize / 2, maxfloor, z - m_mapSize / 2), Quaternion.identity).gameObject.transform.parent = mapObject.transform;
                        }
                        break;
                    case MapState.Goal:
                        //ゴールじゃなかった時は中間地点を設置
                        if (m_mapFloor == maxfloor)
                        {
                            Instantiate(m_goalCubePrefab, new Vector3(x - m_mapSize / 2, maxfloor, z - m_mapSize / 2), Quaternion.identity).gameObject.transform.parent = mapObject.transform;
                            m_goalx = x;
                            m_goalz = z;
                        }
                        else
                        {
                            //次の階につなげるためステータスを変更
                            floormapdata[x, z] = MapState.Road;
                        }
                        break;
                }

                switch (floormapdata[x, z])
                {
                    //指定高さの一つ下に作る（床）
                    case MapState.Wall:
                        Instantiate(m_cubePrefab, new Vector3(x - m_mapSize / 2, maxfloor - 1, z - m_mapSize / 2), Quaternion.identity).gameObject.transform.parent = mapObject.transform;
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