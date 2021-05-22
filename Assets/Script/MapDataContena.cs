using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MapDataContena
{
    [SerializeField] private string _mapDataName;
    [SerializeField] private MapData _mapData;

    public string MapDataName => _mapDataName;
    public MapData MapData => _mapData;

    public MapDataContena(string mapDataName, MapData mapData)
    {
        _mapDataName = mapDataName;
        _mapData = mapData;
    }

    public void SetMapDataName(string name)
    {
        _mapDataName = name;
    }
}

[Serializable]
public class MapDataStore
{
    [SerializeField] private MapDataContena[] _contenas = new MapDataContena[5];
    int _contenaCount = 0;

    public void SetContena(MapDataContena contena)
    {
        if (_contenaCount > _contenas.Length - 1) return;
        _contenas[_contenaCount] = contena;
    }

    public MapDataContena GetContna(int index)
    {
        if (_contenaCount > _contenas.Length - 1) return new MapDataContena("",new MapData(1,1,""));

        return _contenas[index];
    }

    public MapDataContena[] GetContnas => _contenas;
}
