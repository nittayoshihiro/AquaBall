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
    /// <summary>テキスト名前をSettingとする</summary>
    private const string m_textName = "Setting";
    /// <summary>設定の形式変数</summary>
    SettingData m_settingData = null;
    /// <summary>重力をかけるか</summary>
    private bool m_seepRd = false;
    /// <summary>オーディオソース</summary>
    AudioSource m_audioSource = null;
    /// <summary>音量ポリューム</summary>
    [SerializeField]Slider m_volumeSlider = null;

    /// <summary>
    ///データ初期化する前
    /// </summary>
    [RuntimeInitializeOnLoadMethod()]
    static void BeforInit()
    {
        //データがなかったら作成する
        if (!File.Exists(FileController.GetFilePath(m_textName)))
        {
            Debug.Log($"{FileController.GetFilePath(m_textName)}のファイルが見つかりませんでした。ファイルを作ります");
            SettingData settingData = new SettingData(GravityController.ControllerState.Joystick, 0.5f);　//デフォルトを0.5f音量
            Debug.Log(JsonUtility.ToJson(settingData));
            FileController.TextSave(m_textName, JsonUtility.ToJson(settingData));
        }
    }

    private void Start()
    {
        m_settingData = GetSettingLoad;
        m_audioSource = GetComponent<AudioSource>();
        ChangeVolume(m_settingData.volumeMeter);
        m_volumeSlider.value = m_settingData.volumeMeter;
    }

    /// <summary>
    /// プレイヤーの重力を変更（重力を受けるか受けないか）
    /// </summary>
    /// <param name="useGravity">重力の変更値</param>
    private void PlayerGravity(bool useGravity)
    {
        m_seepRd = useGravity;
        //プレイヤーを探して
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            Rigidbody playerRd = player.GetComponent<Rigidbody>();
            StartCoroutine(RigidbodySleep(playerRd));
        }
    }

    /// <summary>
    /// Rigidbodyを止めます
    /// </summary>
    /// <param name="rb">止めたいrigidbody</param>
    public IEnumerator RigidbodySleep(Rigidbody rb)
    {
        while (true)
        {
            rb.Sleep();
            if (m_seepRd)
            {
                yield break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 設定ボタン
    /// </summary>
    public void SettingButton()
    {
        m_settingPanel.SetActive(true);
        m_settingData = GetSettingLoad;
        PlayerGravity(false);
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
    /// 音量をリアルタイムで変更
    /// </summary>
    public void SliderVolume()
    {
        ChangeVolume(m_volumeSlider.value);
    }

    /// <summary>
    /// 音量を変更
    /// </summary>
    private void ChangeVolume(float volume)
    {
        m_settingData.volumeMeter = volume;
        m_audioSource.volume = volume;
    }

    /// <summary>
    /// 音量をなくす
    /// </summary>
    public void VolumeMute()
    {
        ChangeVolume(m_volumeSlider.minValue);//ボリュームをゼロにする
        m_volumeSlider.value = m_settingData.volumeMeter;
    }

    /// <summary>
    /// 音量を最大まであげる
    /// </summary>
    public void VolumeMax()
    {
        ChangeVolume(m_volumeSlider.maxValue);//ボリュームを最大にする
        m_volumeSlider.value = m_settingData.volumeMeter;
    }

    /// <summary>
    /// 閉じるボタン
    /// </summary>
    public void CloseButton()
    {
        m_settingPanel.SetActive(false);
        SettingSave(m_settingData);
        PlayerGravity(true);
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
    /// データを消します（その後アプリを落とします）
    /// </summary>
    public void DataErase()
    {
        File.Delete(FileController.GetFilePath(m_textName));
        File.Delete(FileController.GetFilePath(ResultDataController.m_textName));
        File.Delete(FileController.GetFilePath(MazeMapping.m_textName));
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE_WIN
        Application.Quit();
#elif UNITY_ANDROID
        Application.Quit();
#endif
    }

    /// <summary>
    /// 設定ロードして返す
    /// </summary>
    public SettingData GetSettingLoad
    {
        get
        {
            SettingData settingData = JsonUtility.FromJson<SettingData>(FileController.TextLoad(m_textName));
            return settingData;
        }
    }

    public GravityController.ControllerState GetGravityController => m_settingData.controllerState;
}

/// <summary>
/// 設定データ(Json)形式
/// </summary>
[Serializable]
public class SettingData
{
    /// <summary>操作種類</summary>
    public GravityController.ControllerState controllerState = GravityController.ControllerState.Joystick;
    /// <summary>音量</summary>
    public float volumeMeter = 0.5f;//デフォルトを0.5f音量

    /// <summary>
    /// 初期値設定
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="volume"></param>
    public SettingData(GravityController.ControllerState controller, float volume)
    {
        this.controllerState = controller;
        this.volumeMeter = volume;
    }
}