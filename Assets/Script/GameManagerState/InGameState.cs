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
}
