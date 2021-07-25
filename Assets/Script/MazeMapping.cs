using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// マップを生成する
/// </summary>
public class MazeMapping : MonoBehaviour
{
    /// <summary>マップの名前</summary>
    [SerializeField] string m_mapName = "map";
    /// <summary>マップのサイズ（奇数）</summary>
    [Header("マップサイズ※５以上の奇数を入力")]
    [SerializeField] int m_mapSize;
    /// <summary>作成する階層</summary>
    [SerializeField] int m_mapFloor = 0;//デフォルトは階層なし
    /// <summary>キューブプレハブ</summary>
    [SerializeField] GameObject m_cubePrefab = null;
    /// <summary>床キューブプレハブ</summary>
    [SerializeField] GameObject m_floorCubePrefab = null;
    /// <summary>スタートキューブプレハブ</summary>
    [SerializeField] GameObject m_startCubePrefab = null;
    /// <summary>中間地点キューブプレハブ</summary>
    [SerializeField] GameObject m_middleCubePrefab = null;
    /// <summary>ゴールキューブプレハブ</summary>
    [SerializeField] GameObject m_goalCubePrefab = null;
    /// <summary>プレイヤープレハブ</summary>
    [SerializeField] GameObject m_playerPrefab = null;
    /// <summary>テキストに入力された数字</summary>
    [SerializeField] Text m_numdertext = default;
    /// <summary>穴掘りメソッド</summary>
    DrillingMethod m_drillingMethod = new DrillingMethod();
    /// <summary>ゴール保存</summary>
    int m_goalx, m_goalz;
    /// <summary>マップデータ</summary>
    MapData m_mapData = null;
    /// <summary>マップオブジェクト</summary>
    GameObject m_mapObject = null;
    /// <summary>保存マップ名前形式</summary>
    public const string m_textName = "mapdata";
    /// <summary>マップ保持</summary>
    List<GameObject> m_maps = new List<GameObject>();


    /// <summary>マップを消します(プレイヤーも) </summary>
    public void DeletMap()
    {
        m_maps.ForEach(map => Destroy(map));
        Destroy(GameObject.FindGameObjectWithTag("Player"));
    }

    /// <summary>迷路マップ</summary>
    public void CreateMap()
    {
        FlatMapping();
        //マップデータを入れる
        m_maps.Add(GetMapObject);
    }

    /// <summary>平らな迷路マップ</summary>
    private void FlatMapping()
    {
        m_drillingMethod.ResetMapData(m_mapSize);
        m_drillingMethod.m_mapdata[1, 1] = DrillingMethod.MapState.Start;
        m_drillingMethod.m_goalpoint = true;
        m_drillingMethod.DigHole(new Vector3Int(1, 0, 1));
        CreateFloorMap(m_drillingMethod.GetMapData, m_drillingMethod.m_floormapdata, m_mapName, m_mapFloor);
    }

    /// <summary>マップサイズを変更します</summary>
    private void MapResize()
    {
        if (int.TryParse(m_numdertext.text, out int result))
        {
            if (5 < result && result % 2 == 1)
            {
                m_mapSize = result;
            }
        }
    }

    /// <summary>
    ///　マップのデータを元にフロアマップを作成
    /// </summary>
    /// <param name="mapdata">マップデータ</param>
    /// <param name="mapname">親オブジェクトになる名前d</param>
    /// <param name="maxfloor">作る高さ(maxの高さ)</param>
    private void CreateFloorMap(DrillingMethod.MapState[,] mapdata, DrillingMethod.MapState[,] floormapdata, string mapname, float maxfloor)
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
                    case DrillingMethod.MapState.Wall:
                        cube = Instantiate(m_cubePrefab, new Vector3(x - m_mapSize / 2, maxfloor, z - m_mapSize / 2), Quaternion.identity);
                        cube.gameObject.transform.parent = m_mapObject.transform;
                        break;
                    case DrillingMethod.MapState.Start:
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
                    case DrillingMethod.MapState.Goal:
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
                            floormapdata[x, z] = DrillingMethod.MapState.Road;
                        }
                        break;
                }

                switch (floormapdata[x, z])
                {
                    //指定高さの一つ下に作る（床）
                    case DrillingMethod.MapState.Wall:
                        Instantiate(m_floorCubePrefab, new Vector3(x - m_mapSize / 2, maxfloor - 1, z - m_mapSize / 2), Quaternion.identity).gameObject.transform.parent = m_mapObject.transform;
                        break;
                }
            }
        }
    }

    /// <summary>
    /// マップデータをセーブします
    /// </summary>
    public void MapDataSave()
    {
        m_mapData = new MapData(m_mapSize, m_mapSize, MapdataCode(m_drillingMethod.GetMapData));//FromMapStateConversionToState(m_drillingMethod.GetMapData));
        Debug.Log(m_mapData.IntMapData);
        FileController.TextSave(m_textName, JsonUtility.ToJson(m_mapData));
    }

    /// <summary>
    /// マップデータを取得、そのデータを元にマップを作成
    /// </summary>
    public void LoadAndMapCreate()
    {
        m_mapData = MapDataLoad;
        DrillingMethod.MapState[,] mapState = MapDataCodeToMapState(m_mapData.IntMapData);//FromStringConversionToMapState(m_mapData);
        m_drillingMethod.ResetMapData(m_mapData.x);
        Debug.Log(new { mapState, m_drillingMethod.m_floormapdata, m_mapName, m_mapFloor });
        CreateFloorMap(mapState, m_drillingMethod.m_floormapdata, m_mapName, m_mapFloor);
        //マップデータを入れる
        m_maps.Add(GetMapObject);
    }

    /// <summary>
    /// マップデータをロードします
    /// </summary>
    /// <returns>Mapdataを返します</returns>
    private MapData MapDataLoad
    {
        get
        {
            MapData mapdata = JsonUtility.FromJson<MapData>(FileController.TextLoad(m_textName));
            return mapdata;
        }
    }

    /// <summary>
    /// マップデータコードを返します。
    /// </summary>
    /// <param name="mapState"></param>
    /// <returns></returns>
    public int[] MapdataCode(DrillingMethod.MapState[,] mapState)
    {
        int[] mapdatacode = new int[mapState.GetLength(1)];
        for (int x = 0; x < mapState.GetLength(0); x++)
        {
            int mapSeed = 0;//マップ列内シード
            for (int z = 0; z < mapState.GetLength(1); z++)
            {
                if (mapState[x, z] == DrillingMethod.MapState.Wall)
                {
                    int two = 1;
                    for (int index = 0; index < z; index++)
                    {
                        two *= 2;//２進数の重み
                    }
                    mapSeed += two;
                }
            }
            mapdatacode[x] = mapSeed;
        }
        return mapdatacode;
    }

    /// <summary>
    /// マップコードをマップステータスに変更します。
    /// </summary>
    /// <param name="mapdatacode"></param>
    /// <returns></returns>
    public DrillingMethod.MapState[,] MapDataCodeToMapState(int[] mapdatacode)
    {
        DrillingMethod.MapState[,] mapState = new DrillingMethod.MapState[m_mapSize, m_mapSize];
        int num = 1144;//２進数マップサイズ最大時
        int mapcode = 0;
        for (int i = 0; i < mapdatacode.Length; i++)
        {
            mapcode = mapdatacode[i];
            num = 1024;
            Debug.Log(mapcode);
            for (int j = 0; mapcode != 0; j++)
            {
                if (mapcode >= num)
                {
                    mapcode -= num;
                    mapState[i, j] = DrillingMethod.MapState.Wall;
                }
                else
                {
                    mapState[i, j] = DrillingMethod.MapState.Road;
                }

                if (2 <= num)
                {
                    num /= 2;
                }
                else
                {
                    num -= 1;
                }
                Debug.Log(num + ":" + mapcode);
            }

        }
        return mapState;
    }

    ///// <summary>
    ///// MapState[,]からMapDataに変換します
    ///// </summary>
    ///// <param name="mapState">mapState</param>
    ///// <returns>string形式でマップデータを返します</returns>
    //private string FromMapStateConversionToState(DrillingMethod.MapState[,] mapState)
    //{
    //    string mapdata = default;
    //    for (int x = 0; x < mapState.GetLength(0); x++)
    //    {
    //        for (int z = 0; z < mapState.GetLength(1); z++)
    //        {
    //            mapdata += (int)mapState[x, z];
    //            mapdata += ',';
    //        }
    //        mapdata += '\n';
    //    }
    //    return mapdata;
    //}

    ///// <summary>
    ///// MapDataからMapState[,]に変換します
    ///// </summary>
    ///// <param name="mapData">変換したいMapData</param>
    ///// <returns>マップデータを返します</returns>
    //private DrillingMethod.MapState[,] FromStringConversionToMapState(MapData mapData)
    //{
    //    DrillingMethod.MapState[,] mapState = new DrillingMethod.MapState[mapData.x, mapData.z];
    //    string[] one = mapData.stringMapData.Split('\n');
    //    for (int x = 0; x < mapData.x; x++)
    //    {
    //        string[] two = one[x].Split(',');
    //        for (int z = 0; z < mapData.z; z++)
    //        {
    //            mapState[x, z] = (DrillingMethod.MapState)int.Parse(two[z]);
    //        }
    //    }
    //    return mapState;
    //}

    /// <summary>マップ</summary>
    public GameObject GetMapObject => m_mapObject;
}

/// <summary>
/// マップデータ（Json）形式
/// </summary>
[Serializable]
public class MapData
{
    /// <summary>マップサイズX</summary>
    [SerializeField] private int _x;
    /// <summary>マップサイズZ</summary>
    [SerializeField] private int _z;
    /// <summary>文字列マップデータ</summary>
    [SerializeField] private int[] _intMapData;//string _stringMapData;


    public int x => _x;
    public int z => _z;
    public int[] IntMapData => _intMapData;
    //public string stringMapData => _stringMapData;

    /// <summary>
    /// マップデータ初期化
    /// </summary>
    /// <param name="mapx">マップサイズX</param>
    /// <param name="mapz">マップサイズZ</param>
    /// <param name="mapdata">文字列マップデータ</param>
    public MapData(int mapx, int mapz, int[] mapdata)
    {
        _x = mapx;
        _z = mapz;
        _intMapData = mapdata;
        //_stringMapData = mapdata;
    }
}