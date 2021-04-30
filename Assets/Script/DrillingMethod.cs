using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// マップデータ作成（穴掘り方）
/// </summary>
public class DrillingMethod
{
    /// <summary>マップのサイズ（奇数）</summary>
    [Header("マップサイズ※５以上の奇数を入力")]
    int m_mapSize;
    /// <summary>マップデータ</summary>
    public MapState[,] m_mapdata = null;
    /// <summary>床マップデータ</summary>
    public MapState[,] m_floormapdata = null;
    /// <summary>ゴールを作成するかの判定</summary>
    public bool m_goalpoint = false;
    /// <summary>ゴールポジション</summary>
    private Vector3Int m_goalposition = new Vector3Int();
    /// <summary>掘り進める候補</summary>
    private List<Vector3Int> m_startPos = null;

    /// <summary>
    /// マップデータを初期化（全て壁のデータにする,スタート候補を削除）
    /// </summary>
    public void ResetMapData(int mapSize)
    {
        m_mapSize = mapSize;
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
    public void DigHole(Vector3Int mapPos)
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

    /// <summary>マップデータを返します</summary>
    public MapState[,] GetMapData => m_mapdata;
    /// <summary>ゴールポジションを返します</summary>
    public Vector3Int GetGoalPos => m_goalposition;

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