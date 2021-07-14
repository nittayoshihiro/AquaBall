using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameManagerId
{
    None,
    Initialized,
    InGame,
    Pause
}

/// <summary>
/// ゲームマネージャーのステート抽象クラス
/// </summary>
public abstract class GameManagerBaseState
{
    public GameManagerId gameManagerId { get; private set; } = GameManagerId.None;

    public void SetStateId(GameManagerId setStateId)
    {
        gameManagerId = setStateId;
    }

    /// <summary>
    /// ステートを開始した時に呼ばれる
    /// </summary>
    /// <param name="prevbaseState"></param>
    public virtual void OnEnter(GameManager owner, GameManagerBaseState prevbaseState) { }
    /// <summary>
    /// 毎フレーム呼ばれる
    /// </summary>
    public virtual void OnUpdate(GameManager owner) { }
    /// <summary>
    /// ステートが終わった時に呼ばれる
    /// </summary>
    /// <param name="nextState"></param>
    public virtual void OnExit(GameManager owner, GameManagerBaseState nextState) { }
}
