using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager
{
    /// <summary>
    /// 初期化のステート
    /// </summary>
    public class InitializedState : GameManagerBaseState
    {
        public override void OnEnter(GameManager owner, GameManagerBaseState prevbaseState)
        {
            owner.m_startScreen.SetActive(true);
        }

        public override void OnUpdate(GameManager owner)
        {

        }

        public override void OnExit(GameManager owner, GameManagerBaseState nextState)
        {
            owner.m_startScreen.SetActive(false);
        }
    }
}

