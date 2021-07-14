using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GravityController
{
    /// <summary>
    /// ジョイスティック状態の処理
    /// </summary>
    public class StateJoystick : GravityControllerBaseState
    {
        public override void OnEnter(GravityController owner, GravityControllerBaseState gravityControllerBaseState)
        {

        }

        public override void OnUpdate(GravityController owner)
        {
            //キー入力を検知ベクトルを設定
            owner.m_vector3.x = owner.m_joystick.Horizontal;
            owner.m_vector3.z = owner.m_joystick.Vertical;
            owner.m_vector3.y = m_gravityScaleY;
            //シーンの重力を入力ベクトルの方向に合わせて変化させる
            Physics.gravity = m_gravity * owner.m_vector3.normalized * owner.m_gravityScale;
            if (owner.m_debug)
            {
                Debug.Log(Physics.gravity);
            }
        }

        public override void OnExit(GravityController owner, GravityControllerBaseState nextState)
        {

        }
    }
}
