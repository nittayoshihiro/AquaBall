using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(TimeManager))]
public class GameManager : MonoBehaviour
{
    /// <summary>ゲームの状況</summary>
    private GameState m_gameState = GameState.NonInitialized;
    /// <summary>スタートボタン</summary>
    [SerializeField] GameObject m_startButton;
    /// <summary>終了ボタン</summary>
    [SerializeField] GameObject m_finishButton;
    TimeManager m_timeManager;

    // Start is called before the first frame update
    void Start()
    {
        m_timeManager = GetComponent<TimeManager>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_gameState)
        {
            case GameState.NonInitialized:
                break;
            case GameState.Initialized:
                GameSetUp();
                break;
            case GameState.InGame:
                m_timeManager.TimeNow();
                break;
            case GameState.Pause:
                break;
        }
    }

    /// <summary>
    /// ゲームステータス変更
    /// </summary>
    /// <param name="gameState">変更したいステータス</param>
    public void ChangeGameState(GameState gameState)
    {
        m_gameState = gameState;
    }

    /// <summary>
    /// ゲーム準備
    /// </summary>
    public void Standby()
    {
        ChangeGameState(GameState.Initialized);
        m_startButton.SetActive(false);
    }

    /// <summary>
    /// ゲームをセットアップする
    /// </summary>
    public void GameSetUp()
    {
        ChangeGameState(GameState.InGame);
    }

    /// <summary>設定ボタン</summary>
    public void SettingButton()
    {
        m_timeManager.TimerStop();
        ChangeGameState(GameState.Pause);
        GameObject game = GameObject.Find("Player(Clone)");
        Rigidbody playerrd = game.GetComponent<Rigidbody>();
        playerrd.isKinematic = true;
    }

    /// <summary>閉じるボタン</summary>
    public void ExitButton()
    {
        m_timeManager.TimerStart();
        ChangeGameState(GameState.InGame);
    }

    /// <summary>ゲーム終了</summary>
    public void EndOfGame()
    {
        m_timeManager.TimerStop();
        m_gameState = GameState.NonInitialized;
        m_finishButton.SetActive(true);
    }

    /// <summary>終了ボタン</summary>
    public void FinshButton()
    {
        //子オブジェクトを削除（マップ）
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }
        GameObject player = GameObject.Find("Player(Clone)");
        Destroy(player);
        m_finishButton.SetActive(false);
        m_startButton.SetActive(true);
    }

    public GameState GetGameState => m_gameState;

    /// <summary>
    /// ゲーム状態
    /// </summary>
    public enum GameState
    {
        /// <summary>ゲーム初期化前</summary>
        NonInitialized,
        /// <summary>ゲーム初期化済み、ゲーム開始前</summary>
        Initialized,
        /// <summary>ゲーム中</summary>
        InGame,
        /// <summary>ポーズ中</summary>
        Pause
    }
}