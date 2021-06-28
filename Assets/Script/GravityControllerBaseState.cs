using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステートを管理するためのID
/// </summary>
public enum StateId
{
    None,
    JoyStick,
    Acceleration,
}

/// <summary>
/// 重力操作のステート抽象クラス
/// </summary>
public abstract class GravityControllerBaseState
{
    public StateId StateId { get; private set; } = StateId.None;

    public void SetStateId(StateId stateId)
    {
        StateId = stateId;
    }

    /// <summary>
    /// ステートを開始した時に呼ばれる
    /// </summary>
    /// <param name="prevbaseState"></param>
    public virtual void OnEnter(GravityController owner, GravityControllerBaseState prevbaseState) { }
    /// <summary>
    /// 毎フレーム呼ばれる
    /// </summary>
    public virtual void OnUpdate(GravityController owner) { }
    /// <summary>
    /// ステートが終わった時に呼ばれる
    /// </summary>
    /// <param name="nextState"></param>
    public virtual void OnExit(GravityController owner, GravityControllerBaseState nextState) { }
}
