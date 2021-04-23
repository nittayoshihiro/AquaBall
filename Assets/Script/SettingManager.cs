using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 設定用クラス
/// </summary>
public class SettingManager : MonoBehaviour
{
    /// <summary>ゲームマネージャー</summary>
    GameManager m_gameManager;
    /// <summary>設定画面のオブジェクト</summary>
    [SerializeField] GameObject m_settingPanel = null;
    /// <summary>コントロールボタン</summary>
    [SerializeField] GameObject m_button = null;
    /// <summary>ボタンテキスト</summary>
    [SerializeField] Text m_buttonText;
    /// <summary>テキスト名前をSettingとする</summary>
    static string m_textName = "Setting";
    /// <summary>設定の形式変数</summary>
    SettingData m_settingData = null;

    /// <summary>
    ///データ初期化する前
    /// </summary>
    [RuntimeInitializeOnLoadMethod()]
    static void BeforInit()
    {
        //データがあったら、
        try
        {
            using (var reader = new StreamReader(FileController.GetFilePath(m_textName))) { }
        }
        catch (FileNotFoundException ex)
        {
            Debug.Log($"{ex}のファイルが見つかりませんでした。ファイルを作ります");
            SettingData settingData = new SettingData(GravityController.ControllerState.Joystick);
            Debug.Log(JsonUtility.ToJson(settingData));
            FileController.TextSave(m_textName, JsonUtility.ToJson(settingData));
        }
    }

    private void Start()
    {
        m_settingData = GetSettingLoad;
    }

    /// <summary>
    /// 設定ボタン
    /// </summary>
    public void SettingButton()
    {
        m_settingPanel.SetActive(true);
        m_settingData = GetSettingLoad;
    }

    /// <summary>
    /// GravityControllerを変更する際呼び出す
    /// </summary>
    /// <param name="controllerState">変更したいコントロール</param>
    public void ChangeGravityController(GravityController.ControllerState controllerState)
    {
        m_settingData.controllerState = controllerState;
    }

    /// <summary>
    /// 閉じるボタン
    /// </summary>
    public void CloseButton()
    {
        m_settingPanel.SetActive(false);
        SettingSave(m_settingData);
    }

    /// <summary>
    /// 設定セーブ
    /// </summary>
    /// <param name="settingData"></param>
    public void SettingSave(SettingData settingData)
    {
        FileController.TextSave(m_textName, JsonUtility.ToJson(settingData));
    }

    /// <summary>
    /// 設定ロードして返す
    /// </summary>
    public SettingData GetSettingLoad
    {
        get
        {
            SettingData settingData = JsonUtility.FromJson<SettingData>(FileController.TextLoad(m_textName));
            Debug.Log(settingData);
            return settingData;
        }
    }

    public GravityController.ControllerState GetGravityController  => m_settingData.controllerState;
}

/// <summary>
/// 設定データ(Json)形式
/// </summary>
[Serializable]
public class SettingData
{
    /// <summary>操作種類</summary>
    public GravityController.ControllerState controllerState = GravityController.ControllerState.Joystick;
    public SettingData(GravityController.ControllerState controller)
    {
        controllerState = controller;
    }
}