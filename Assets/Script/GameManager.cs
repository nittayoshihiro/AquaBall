using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TimeManager))]
[RequireComponent(typeof(GravityController))]
public class GameManager : MonoBehaviour
{
    /// <summary>ゲームの状況</summary>
    private GameState m_gameState = GameState.NonInitialized;
    /// <summary>スタートスクリーン</summary>
    [SerializeField] GameObject m_startScreen= null;
    /// <summary>タイマーテキストオブジェクト</summary>
    [SerializeField] GameObject m_timerText= null;
    /// <summary>結果パネル</summary>
    [SerializeField] GameObject m_resultPanel = null;
    /// <summary>タイム管理</summary>
    TimeManager m_timeManager = null;
    /// <summary>重力コントロール</summary>
    GravityController m_gravityController = null;
    ResultDataController m_resultDataController = null;

    // Start is called before the first frame update
    void Start()
    {
        m_timeManager = GetComponent<TimeManager>();
        m_gravityController = GetComponent<GravityController>();
        m_resultDataController = FindObjectOfType<ResultDataController>();
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
                m_gravityController.Controller();
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
        m_gravityController.ResetGravity();
        ChangeGameState(GameState.Initialized);
        m_startScreen.SetActive(false);
    }

    /// <summary>
    /// ゲームをセットアップする
    /// </summary>
    public void GameSetUp()
    {
        ChangeGameState(GameState.InGame);
        m_timerText.SetActive(true);
        m_gravityController.JoystickJudgment();
    }

    /// <summary>ゲーム終了</summary>
    public void EndOfGame()
    {
        m_timerText.SetActive(false);
        m_timeManager.TimerStop();
        ChangeGameState(GameState.NonInitialized);
        m_gravityController.JoystickJudgment();
        m_resultPanel.SetActive(true);
        m_resultDataController.RankingText(m_timeManager.GetTime);
    }

    /// <summary>終了ボタン</summary>
    public void FinshButton()
    {
        //子オブジェクトを削除（マップ）
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Destroy(player);
        ChangeGameObjectSetActive(m_resultPanel,m_startScreen);
    }

    /// <summary>
    /// ゲームオブジェクトのセットアクティブを変更
    /// </summary>
    /// <param name="falseGameObject">セットアクティブをfaleにしたいオブジェクト</param>
    /// <param name="trueGameObject">セットアクティブをtrueにしたいオブジェクト</param>
    public static void ChangeGameObjectSetActive(GameObject falseGameObject, GameObject trueGameObject)
    {
        falseGameObject.SetActive(false);
        trueGameObject.SetActive(true);
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
        Pause,
    }
}