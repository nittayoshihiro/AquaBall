using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ステート抽象クラス
/// </summary>
public abstract class BaseState
{
    /// <summary>
    /// ステートを開始した時に呼ばれる
    /// </summary>
    /// <param name="prevbaseState"></param>
    public virtual void OnEnter(PlayerController owner, BaseState prevbaseState) { }
    /// <summary>
    /// 毎フレーム呼ばれる
    /// </summary>
    public virtual void OnUpdate(PlayerController owener) { }
    /// <summary>
    /// ステートが終わった時に呼ばれる
    /// </summary>
    /// <param name="nextState"></param>
    public virtual void OnExit(PlayerController owner, BaseState nextState) { }
}
