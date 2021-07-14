using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager
{
    /// <summary>
    /// ゲーム中のステート
    /// </summary>
    public class InGame : GameManagerBaseState
    {
        public override void OnEnter(GameManager owner, GameManagerBaseState prevbaseState)
        {
            owner.GameSetUp();
        }

        public override void OnUpdate(GameManager owner)
        {
            owner.m_gravityController.OnUpdate();
            owner.m_timeManager.TimeNow();
        }

        public override void OnExit(GameManager owner, GameManagerBaseState nextState)
        {

        }
    }


    /// <summary>
    /// ゲームをセットアップする
    /// </summary>
    public void GameSetUp()
    {
        Debug.Log("setUp");
        m_timerText.SetActive(true);
        m_gravityController.JoystickJudgment();
    }
}
