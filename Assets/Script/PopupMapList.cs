
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// マップリストをポップアップするクラス
/// </summary>
public class PopupMapList : MonoBehaviour
{
    [SerializeField] Text m_title;
    [SerializeField] Dropdown m_dropdown;
    [SerializeField] Button m_deletMapButton;
    [SerializeField] Button m_loadMapButton;
    [SerializeField] Button m_createMapButton;
    [SerializeField] Button m_popupClose;
    MapDataStore m_mapDataStore = null;

    public void Initialize(string title, string[] mapnames, Action mapLoadCallback, Action closeCallback)
    {
        m_title.text = title;
        m_dropdown.options = new List<Dropdown.OptionData>();
        for (int i = 0; i < mapnames.Length; i++)
        {
            MapDataContena mapDataContena = JsonUtility.FromJson<MapDataContena>(FileController.TextLoad(mapnames[i]));
            Dropdown.OptionData optionData = new Dropdown.OptionData();
            optionData.text = mapDataContena.MapDataName;
            m_dropdown.options.Add(optionData);
        }

        m_loadMapButton.onClick.AddListener(() => mapLoadCallback?.Invoke());
        m_popupClose.onClick.AddListener(() => closeCallback?.Invoke());
    }

    /// <summary>
    /// ドロップダウンで選択されたマップを表示します。
    /// </summary>
    public void SelectMapLoad()
    {
        MapDataStore mapDataStore = JsonUtility.FromJson<MapDataStore>(FileController.TextLoad("mapdata"));
        MapDataContena[] mapDataContena = mapDataStore.GetContnas;
        for (int i = 0; i < mapDataContena.Length; i++)
        {
            //選択されたマップ名の時
            if (mapDataContena[i].MapDataName == m_dropdown.options[m_dropdown.value].text)
            {
                //mapDataContena[i];
            }
        }
    }

    /// <summary>
    /// マップを作るボタン
    /// </summary>
    public void CreateButton()
    {

    }

    /// <summary>
    /// マップ消去ボタン
    /// </summary>
    public void DeletButton()
    {
        m_mapDataStore.DeletContena(m_dropdown.value);
    }
}
