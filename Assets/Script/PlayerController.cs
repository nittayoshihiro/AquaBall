using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの操作を管理する
/// </summary>
public class PlayerController : MonoBehaviour
{
    //ステートのインスタンス
    private static readonly PlayerDefaltState playerDefaltState = new PlayerDefaltState();
    private static readonly PlayerJumpState playerJumpState = new PlayerJumpState();

    /// <summary>
    /// 現在のステータス
    /// </summary>
    private BaseState nowState = new PlayerDefaltState();

    private void Start()
    {
        nowState.OnEnter(this,null);
    }

    private void Update()
    {
        nowState.OnUpdate(this);
    }

    /// <summary>
    /// ステート変更
    /// </summary>
    /// <param name="nextState"></param>
    private void ChangeState(BaseState nextState)
    {
        nowState.OnExit(this,nextState);
        nextState.OnEnter(this, nowState);
        nowState = nextState;
    }

    public class PlayerDefaltState : BaseState
    {
        public override void OnEnter(PlayerController owner, BaseState prevbaseState)
        {

        }

        public override void OnUpdate(PlayerController owener)
        {

        }

        public override void OnExit(PlayerController owner, BaseState nextState)
        {

        }
    }

    public class PlayerJumpState : BaseState
    {
        public override void OnEnter(PlayerController owner, BaseState prevbaseState)
        {

        }

        public override void OnUpdate(PlayerController owener)
        {

        }

        public override void OnExit(PlayerController owner, BaseState nextState)
        {

        }
    }
}