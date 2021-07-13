using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GravityController
{
    /// <summary>
    /// 加速度センサー状態の処理
    /// </summary>
    public class StateAcceleration : GravityControllerBaseState
    {
        public override void OnEnter(GravityController owner, GravityControllerBaseState prevbaseState)
        {

        }

        public override void OnUpdate(GravityController owner)
        {
#if UNITY_EDITOR||UNITY_STANDALONE_WIN||UNITY_STANDALONE_OSX
            //キー入力を検知ベクトルを設定
            owner.m_vector3.x = Input.GetAxis("Horizontal") * owner.m_strongAcceleration;
            owner.m_vector3.z = Input.GetAxis("Vertical") * owner.m_strongAcceleration;
            owner.m_vector3.y = m_gravityScaleY;
#elif UNITY_ANDROID
            //スマホデバッグ用
            //加速度センサーの入力をUnity空間の軸にマッピングする(座標軸が異なるため)
            owner.m_vector3.x = Input.acceleration.x;
            owner.m_vector3.z = Input.acceleration.y;
            owner.m_vector3.y = m_gravityScaleY;//マップ外に行かないようにする
#endif
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
