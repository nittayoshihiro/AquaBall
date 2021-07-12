using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームマネージャーのステート抽象クラス
/// </summary>
public abstract class GameManagerBaseState
{
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
