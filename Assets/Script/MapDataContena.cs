using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// マップの入れ物
/// </summary>
[Serializable]
public class MapDataContena
{
    /// <summary>マップの名前</summary>
    [SerializeField] private string m_mapDataNamem;
    /// <summary>マップデータ</summary>
    [SerializeField] private MapData m_mapData;

    public string MapDataName => m_mapDataNamem;
    public MapData MapData => m_mapData;

    public MapDataContena(string mapDataName, MapData mapData)
    {
        m_mapDataNamem = mapDataName;
        m_mapData = mapData;
    }

    public void SetMapDataName(string name)
    {
        m_mapDataNamem = name;
    }
}

/// <summary>
/// マップ格納
/// </summary>
[Serializable]
public class MapDataStore
{
    /// <summary>複数のコンテナマップデータ</summary>
    [SerializeField] private MapDataContena[] m_contenas = new MapDataContena[5];
    /// <summary></summary>
    int m_contenaCount = 0;

    public void SetContena(MapDataContena contena)
    {
        if (m_contenaCount > m_contenas.Length - 1) return;
        m_contenas[m_contenaCount] = contena;
    }

    public MapDataContena GetContna(int index)
    {
        if (m_contenaCount > m_contenas.Length - 1) return new MapDataContena("",new MapData(1,1,""));

        return m_contenas[index];
    }

    public MapDataContena[] GetContnas => m_contenas;
}
