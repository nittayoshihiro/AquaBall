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
    [Header("マップサイズ※５以上の奇数を入力")]
    [SerializeField] int m_mapSize;
    /// <summary>作成する階層</summary>
    [SerializeField] int m_mapFloor;
    /// <summary>キューブプレハブ</summary>
    [SerializeField] GameObject m_cubePrefab = null;
    /// <summary>スタートキューブプレハブ</summary>
    [SerializeField] GameObject m_startCubePrefab = null;
    /// <summary>中間地点キューブプレハブ</summary>
    [SerializeField] GameObject m_middleCubePrefab = null;
    /// <summary>ゴールキューブプレハブ</summary>
    [SerializeField] GameObject m_goalCubePrefab = null;
    /// <summary>プレイヤープレハブ</summary>
    [SerializeField] GameObject m_playerPrefab = null;
    /// <summary>デバッグonoff</summary>
    [SerializeField] bool m_debug = true;
    /// <summary>マップデータ</summary>
    private MapState[,] m_mapdata;
    /// <summary>ゴールを作成するかの判定</summary>
    private bool m_goalpoint;
    private List<Vector3Int> m_startPos = null;
    //[SerializeField] int m_startx, m_startz;
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
        ResetMapData();
        Debug.Log(m_mapdata.GetLength(0) + " " + m_mapdata.GetLength(1));
        m_mapdata[1, 1] = MapState.Road;
        DigHole(new Vector3Int(1, 0, 1));
        CreateFloorMap(m_mapdata, m_mapdata, m_mapName, m_mapFloor);
    }

    // Start is called before the first frame update
    public void MappingStart()
    {

    }

    /// <summary>
    /// マップデータを初期化（全て壁のデータにする）
    /// </summary>
    private void ResetMapData()
    {
        m_mapdata = new MapState[m_mapSize, m_mapSize];
        for (int x = 0; x < m_mapSize; x++)
        {
            for (int z = 0; z < m_mapSize; z++)
            {
                m_mapdata[x, z] = MapState.Wall;
            }
        }
    }

    /// <summary>
    /// 穴を掘る
    /// </summary>
    /// <param name="mapPos"></param>
    public void DigHole(Vector3Int mapPos)
    {
        while (true)
        {
            //上下左右で進める数字を保存する（掘れる所 上１ 下２ 左３ 右４）
            int[] ramDirection = new int[] { 0 };
            if (mapPos.z + 2 < m_mapSize)
            {
                //Debug.Log(mapPos.z + 2);
                if (m_mapdata[mapPos.x, mapPos.z + 2] == MapState.Wall)
                {
                    System.Array.Resize(ref ramDirection, ramDirection.Length + 1);
                    ramDirection[ramDirection.Length - 1] = 1;
                }
            }
            if (0 < mapPos.z - 2)
            {
                //Debug.Log(mapPos.z - 2);
                if (m_mapdata[mapPos.x, mapPos.z - 2] == MapState.Wall)
                {
                    System.Array.Resize(ref ramDirection, ramDirection.Length + 1);
                    ramDirection[ramDirection.Length - 1] = 2;
                }
            }
            if (0 < mapPos.x - 2)
            {
                //Debug.Log(mapPos.x - 2);
                if (m_mapdata[mapPos.x - 2, mapPos.z] == MapState.Wall)
                {
                    System.Array.Resize(ref ramDirection, ramDirection.Length + 1);
                    ramDirection[ramDirection.Length - 1] = 3;
                }
            }
            if (mapPos.x + 2 < m_mapSize)
            {
                //Debug.Log(mapPos.x + 2);
                if (m_mapdata[mapPos.x + 2, mapPos.z] == MapState.Wall)
                {
                    System.Array.Resize(ref ramDirection, ramDirection.Length + 1);
                    ramDirection[ramDirection.Length - 1] = 4;
                }
            }
            //掘り進められなかったら
            if (ramDirection.Length == 1)
            {
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