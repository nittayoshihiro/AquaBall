using System.IO;
using UnityEngine;

/// <summary>
/// ゲームマネージャー
/// </summary>
[RequireComponent(typeof(TimeManager))]
[RequireComponent(typeof(GravityController))]
public partial class GameManager :SingletonMonoBehaviour<GameManager>
{
    /// <summary>ゲームの状況</summary>
    GameManagerBaseState m_currentGameManagerBaseState;
    /// <summary>初期化ステート</summary>
    InitializedState m_initializedState = new InitializedState();
    /// <summary>ゲーム中ステート</summary>
    InGame m_inGame = new InGame();
    /// <summary>中断ステート</summary>
    PauseState m_pauseState = new PauseState();
    /// <summary>スタートスクリーン</summary>
    [SerializeField] GameObject m_startScreen = null;
    /// <summary>マップロードボタン</summary>
    [SerializeField] GameObject m_LoadButton = null;
    /// <summary>タイマーテキストオブジェクト</summary>
    [SerializeField] GameObject m_timerText = null;
    /// <summary>結果パネル</summary>
    [SerializeField] GameObject m_resultPanel = null;
    /// <summary>タイム管理</summary>
    TimeManager m_timeManager = null;
    /// <summary>重力コントロール</summary>
    GravityController m_gravityController = null;
    /// <summary>結果データ</summary>
    ResultDataController m_resultDataController = null;

    /// <summary>今のゲーム状態を取得する</summary>
    public GameManagerBaseState CurrentState => m_currentGameManagerBaseState;

    private void Awake()
    {
        //1つ以上あったらゲームオブジェクトを削除
        if (this != Instance)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_currentGameManagerBaseState = m_initializedState;
        m_timeManager = GetComponent<TimeManager>();
        m_gravityController = GetComponent<GravityController>();
        m_resultDataController = FindObjectOfType<ResultDataController>();

        //ステートのIDを設定
        m_initializedState.SetStateId(GameManagerId.Initialized);
        m_inGame.SetStateId(GameManagerId.InGame);
        m_pauseState.SetStateId(GameManagerId.Pause);

        if (File.Exists(FileController.GetFilePath(MazeMapping.m_textName)))
        {
            m_LoadButton.SetActive(true);
        }
        else
        {
            m_LoadButton.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_currentGameManagerBaseState.OnUpdate(this);
    }

    /// <summary>
    /// ゲームステータス変更
    /// </summary>
    /// <param name="gameState">変更したいステータス</param>
    public void ChangeGameState(GameManagerBaseState nextGameState)
    {
        m_currentGameManagerBaseState.OnExit(this,nextGameState);
        m_currentGameManagerBaseState = nextGameState;
        m_currentGameManagerBaseState.OnEnter(this, m_currentGameManagerBaseState);
    }

    /// <summary>
    /// ゲーム準備
    /// </summary>
    public void Standby()
    {
        m_gravityController.ResetGravity();
        ChangeGameState(m_inGame);
        m_startScreen.SetActive(false);
    }

    /// <summary>ゲーム終了</summary>
    public void EndOfGame()
    {
        m_timerText.SetActive(false);
        m_timeManager.TimerStop();
        ChangeGameState(m_initializedState);
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
        ChangeGameObjectSetActive(m_resultPanel, m_startScreen);
        Debug.Log(File.Exists(FileController.GetFilePath(MazeMapping.m_textName)));
        if (File.Exists(FileController.GetFilePath(MazeMapping.m_textName)))
        {
            m_LoadButton.SetActive(true);
        }
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
}