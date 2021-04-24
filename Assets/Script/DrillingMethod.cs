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
    string m_mapName = "map";
    /// <summary>マップのサイズ（奇数）</summary>
    [Header("マップサイズ※５以上の奇数を入力")]
    int m_mapSize;
    /// <summary>作成する階層</summary>
    int m_mapFloor = 0;
    GameObject m_mapObject = null;
    /// <summary>キューブプレハブ</summary>
    GameObject m_cubePrefab = null;
    /// <summary>床キューブプレハブ</summary>
    GameObject m_floorCubePrefab = null;
    /// <summary>スタートキューブプレハブ</summary>
    GameObject m_startCubePrefab = null;
    /// <summary>中間地点キューブプレハブ</summary>
    GameObject m_middleCubePrefab = null;
    /// <summary>ゴールキューブプレハブ</summary>
    GameObject m_goalCubePrefab = null;
    /// <summary>プレイヤープレハブ</summary>
    GameObject m_playerPrefab = null;
    /// <summary>デバッグ用</summary>
    [SerializeField] bool m_debug = false;
    /// <summary>マップデータ</summary>
    private MapState[,] m_mapdata = null;
    /// <summary>床マップデータ</summary>
    private MapState[,] m_floormapdata;
    /// <summary>ゴールを作成するかの判定</summary>
    private bool m_goalpoint = false;
    /// <summary>ゴールポジション</summary>
    private Vector3Int m_goalposition = new Vector3Int();
    /// <summary>掘り進める候補</summary>
    private List<Vector3Int> m_startPos = null;
    /// <summary>ゴール保存</summary>
    int m_goalx, m_goalz;

    /// <summary>
    /// マッピングテスト用
    /// </summary>
    public void TestMapping()
    {
        m_startPos = new List<Vector3Int>();
        //子オブジェクトを削除（マップ）
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }
        GameObject player = GameObject.Find("Player(Clone)");
        Destroy(player);

        //マップ生成
        ResetMapData();
        if (m_debug)
        {
            Debug.Log(m_mapdata.GetLength(0) + " " + m_mapdata.GetLength(1));
        }

        m_mapdata[1, 1] = MapState.Start;
        DigHole(new Vector3Int(1, 0, 1));
        CreateFloorMap(m_mapdata, m_floormapdata, m_mapName, m_mapFloor);
    }

    /// <summary>
    /// 平面のマップを生成します
    /// </summary>
    /// <param name="mapName">マップの名前</param>
    /// <param name="mapSize">マップサイズ</param>
    /// <param name="cubePrefab">キューブプレハブ</param>
    /// <param name="floorCubePrefab">床キューブプレハブ</param>
    /// <param name="startCubePrefab">スタートキューブプレハブ</param>
    /// <param name="middleCubePrefab">中間地点キューブプレハブ</param>
    /// <param name="goalCubePrefab">ゴールキューブプレハブ</param>
    /// <param name="playerPrefab">プレイヤープレハブ</param>
    public DrillingMethod(string mapName, int mapSize, GameObject cubePrefab, GameObject floorCubePrefab, GameObject startCubePrefab,
        GameObject middleCubePrefab, GameObject goalCubePrefab, GameObject playerPrefab)
    {
        m_mapName = mapName;
        m_mapSize = mapSize;
        m_cubePrefab = cubePrefab;
        m_floorCubePrefab = floorCubePrefab;
        m_startCubePrefab = startCubePrefab;
        m_middleCubePrefab = middleCubePrefab;
        m_goalCubePrefab = goalCubePrefab;
        m_playerPrefab = playerPrefab;

    }

    /// <summary>平らな迷路マップ</summary>
    public void FlatMapping()
    {
        ResetMapData();
        m_mapdata[1, 1] = MapState.Start;
        m_goalpoint = true;
        DigHole(new Vector3Int(1, 0, 1));
        CreateFloorMap(m_mapdata, m_floormapdata, m_mapName, 0);
    }

    /// <summary>
    /// 階層型のマップを生成します。
    /// </summary>
    /// <param name="mapName">マップの名前</param>
    /// <param name="cubePrefab">キューブプレハブ</param>
    /// <param name="floorCubePrefab">床キューブプレハブ</param>
    /// <param name="startCubePrefab">スタートキューブプレハブ</param>
    /// <param name="middleCubePrefab">中間地点キューブプレハブ</param>
    /// <param name="goalCubePrefab">ゴールキューブプレハブ</param>
    /// <param name="playerPrefab">プレイヤープレハブ</param>
    /// <param name="floor">階層の範囲</param>
    public DrillingMethod(string mapName, GameObject cubePrefab, GameObject floorCubePrefab, GameObject startCubePrefab,
        GameObject middleCubePrefab, GameObject goalCubePrefab, GameObject playerPrefab, int floor)
    {
        ResetMapData();
        m_mapdata[1, 1] = MapState.Start;
        DigHole(new Vector3Int(1, 0, 1));
        CreateFloorMap(m_mapdata, m_floormapdata, mapName, floor);
        ResetMapData();

        for (int i = 0; i < floor; i++)
        {
            DigHole(GetGoalPos);
            CreateFloorMap(m_mapdata, m_floormapdata, mapName, i);
        }

    }

    /// <summary>
    /// マップデータを初期化（全て壁のデータにする,スタート候補を削除）
    /// </summary>
    private void ResetMapData()
    {
        m_startPos = new List<Vector3Int>();
        m_mapdata = new MapState[m_mapSize, m_mapSize];
        m_floormapdata = new MapState[m_mapSize, m_mapSize];
        for (int x = 0; x < m_mapSize; x++)
        {
            for (int z = 0; z < m_mapSize; z++)
            {
                m_mapdata[x, z] = MapState.Wall;
                m_floormapdata[x, z] = MapState.Wall;
            }
        }
    }

    /// <summary>
    /// 穴を掘る
    /// </summary>
    /// <param name="mapPos"></param>
    private void DigHole(Vector3Int mapPos)
    {
        while (true)
        {
            //上下左右で進める数字を保存する（掘れる所 上１ 下２ 左３ 右４）
            int[] ramDirection = new int[] { 0 };
            if (mapPos.z + 2 < m_mapSize)
            {
                if (m_mapdata[mapPos.x, mapPos.z + 2] == MapState.Wall)
                {
                    System.Array.Resize(ref ramDirection, ramDirection.Length + 1);
                    ramDirection[ramDirection.Length - 1] = 1;
                }
            }
            if (0 < mapPos.z - 2)
            {
                if (m_mapdata[mapPos.x, mapPos.z - 2] == MapState.Wall)
                {
                    System.Array.Resize(ref ramDirection, ramDirection.Length + 1);
                    ramDirection[ramDirection.Length - 1] = 2;
                }
            }
            if (0 < mapPos.x - 2)
            {
                if (m_mapdata[mapPos.x - 2, mapPos.z] == MapState.Wall)
                {
                    System.Array.Resize(ref ramDirection, ramDirection.Length + 1);
                    ramDirection[ramDirection.Length - 1] = 3;
                }
            }
            if (mapPos.x + 2 < m_mapSize)
            {
                if (m_mapdata[mapPos.x + 2, mapPos.z] == MapState.Wall)
                {
                    System.Array.Resize(ref ramDirection, ramDirection.Length + 1);
                    ramDirection[ramDirection.Length - 1] = 4;
                }
            }
            //掘り進められなかったら
            if (ramDirection.Length == 1)
            {
                if (m_goalpoint)
                {
                    m_mapdata[mapPos.x, mapPos.z] = MapState.Goal;
                    m_goalpoint = false;
                }
                ReStartDigHole();
                break;
            }
            else
            {
                switch (ramDirection[Random.Range(1, ramDirection.Length)])
                {
                    case 1:
                        m_mapdata[mapPos.x, ++mapPos.z] = MapState.Road;
                        m_mapdata[mapPos.x, ++mapPos.z] = MapState.Road;
                        m_startPos.Add(new Vector3Int(mapPos.x, 0, mapPos.z));
                        break;
                    case 2:
                        m_mapdata[mapPos.x, --mapPos.z] = MapState.Road;
                        m_mapdata[mapPos.x, --mapPos.z] = MapState.Road;
                        m_startPos.Add(new Vector3Int(mapPos.x, 0, mapPos.z));
                        break;
                    case 3:
                        m_mapdata[--mapPos.x, mapPos.z] = MapState.Road;
                        m_mapdata[--mapPos.x, mapPos.z] = MapState.Road;
                        m_startPos.Add(new Vector3Int(mapPos.x, 0, mapPos.z));
                        break;
                    case 4:
                        m_mapdata[++mapPos.x, mapPos.z] = MapState.Road;
                        m_mapdata[++mapPos.x, mapPos.z] = MapState.Road;
                        m_startPos.Add(new Vector3Int(mapPos.x, 0, mapPos.z));
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 掘れる可能性のあるポジションからランダムで堀る関数
    /// </summary>
    private void ReStartDigHole()
    {
        if (m_startPos.Count != 0)
        {
            int ramIndex = Random.Range(0, m_startPos.Count);
            //Debug.Log(ramIndex + " " + m_startPos.Count);
            Vector3Int startPos = m_startPos[ramIndex];
            m_startPos.RemoveAt(ramIndex);
            DigHole(startPos);
        }
    }

    /// <summary>
    ///　マップのデータを元にフロアマップを作成
    /// </summary>
    /// <param name="mapdata">マップデータ</param>
    /// <param name="mapname">親オブジェクトになる名前d</param>
    /// <param name="maxfloor">作る高さ(maxの高さ)</param>
    private void CreateFloorMap(MapState[,] mapdata, MapState[,] floormapdata, string mapname, float maxfloor)
    {
        //親になるオブジェクトを生成
        m_mapObject = new GameObject(mapname);
        //一度、変数に置く
        GameObject cube = null;
        for (int x = 0; x < mapdata.GetLength(0); x++)
        {
            for (int z = 0; z < mapdata.GetLength(1); z++)
            {
                //親の子オブジェクトとして生成
                switch (mapdata[x, z])
                {
                    case MapState.Wall:
                        cube = Instantiate(m_cubePrefab, new Vector3(x - m_mapSize / 2, maxfloor, z - m_mapSize / 2), Quaternion.identity);
                        cube.gameObject.transform.parent = m_mapObject.transform;
                        break;
                    case MapState.Start:
                        //最初だけ作成※マジックナンバー
                        if (0 == maxfloor)
                        {
                            cube = Instantiate(m_startCubePrefab, new Vector3(x - m_mapSize / 2, maxfloor, z - m_mapSize / 2), Quaternion.identity);
                            cube.transform.parent = m_mapObject.transform;
                            Instantiate(m_playerPrefab, new Vector3(x - m_mapSize / 2, maxfloor, z - m_mapSize / 2), Quaternion.identity);
                        }
                        else
                        {
                            cube = Instantiate(m_middleCubePrefab, new Vector3(x - m_mapSize / 2, maxfloor, z - m_mapSize / 2), Quaternion.identity);
                            cube.transform.parent = m_mapObject.transform;
                        }
                        break;
                    case MapState.Goal:
                        //ゴールじゃなかった時は中間地点を設置
                        if (m_mapFloor == maxfloor)
                        {
                            cube = Instantiate(m_goalCubePrefab, new Vector3(x - m_mapSize / 2, maxfloor, z - m_mapSize / 2), Quaternion.identity);
                            cube.transform.parent = m_mapObject.transform;
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
                        Instantiate(m_floorCubePrefab, new Vector3(x - m_mapSize / 2, maxfloor - 1, z - m_mapSize / 2), Quaternion.identity).gameObject.transform.parent = m_mapObject.transform;
                        break;
                }
            }
        }
    }

    /// <summary>マップデータを返します</summary>
    public MapState[,] GetMapData => m_mapdata;
    /// <summary>ゴールポジションを返します</summary>
    public Vector3Int GetGoalPos => m_goalposition;
    /// <summary>マップ</summary>
    public GameObject GetMapObject => m_mapObject;


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