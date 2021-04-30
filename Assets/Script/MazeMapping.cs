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
    DrillingMethod m_drillingMethod = null;
    /// <summary>ゴール保存</summary>
    int m_goalx, m_goalz;
    /// <summary>マップオブジェクト</summary>
    GameObject m_mapObject = null;
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
        //TODODrillingMethodにMonoBehaviourを継承しているのをnewしてはいけない（結果　コンストラクタはつかえない？）
        m_drillingMethod = new DrillingMethod();
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

    /// <summary>マップ</summary>
    public GameObject GetMapObject => m_mapObject;
}