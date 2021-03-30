using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの操作を管理する
/// </summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>重力</summary>
    const float m_gravity = 9.81f;
    /// <summary>重力をかける方向</summary>
    Vector3 m_vector3 = new Vector3();
    /// <summary>重力規模</summary>
    [SerializeField] float m_gravityScale = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //unityエディター上で動かしている場合
        if (Application.isEditor)
        {
            //キー入力を検知ベクトルを設定
            m_vector3.x = Input.GetAxis("Horizontal");
            m_vector3.z = Input.GetAxis("Vertical");
            if(Input.GetKey("z"))
            {
                m_vector3.y = -1.0f;
            }
        }
        //スマホデバッグ用
        else
        {
            //加速度センサーの入力をUnity空間の軸にマッピングする(座標軸が異なるため)
            m_vector3.x = Input.acceleration.x;
            m_vector3.z = Input.acceleration.y;
            m_vector3.y = Input.acceleration.z;
        }
        //シーンの重力を入力ベクトルの方向に合わせて変化させる
        Physics.gravity = m_gravity * m_vector3.normalized * m_gravityScale;
        //Debug.Log(Physics.gravity);
    }
}
