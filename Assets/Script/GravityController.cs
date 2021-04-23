using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 重力の操作をします。
/// </summary>
public class GravityController : MonoBehaviour
{
    /// <summary>重力</summary>
    const float m_gravity = 9.81f;
    /// <summary>重力をかける方向</summary>
    Vector3 m_vector3 = new Vector3();
    /// <summary>ジョイスティックゲームオブジェクト</summary>
    [SerializeField] GameObject m_joystickGameObject =null;
    /// <summary>FloatJoystick</summary>
    FloatingJoystick m_joystick;
    /// <summary>重力規模</summary>
    [SerializeField] float m_gravityScale = 1.0f;
    /// <summary>デバッグonoff</summary>
    [SerializeField] bool m_debug = true;
    /// <summary>設定</summary>
    SettingManager m_settingManager = null;

    private void Start()
    {
        m_joystick = m_joystickGameObject.GetComponent<FloatingJoystick>();
        m_settingManager = FindObjectOfType<SettingManager>();
        Debug.Log(m_settingManager);
    }

    /// <summary>
    /// コントロール
    /// </summary>
    public void Controller()
    {
        //unityエディター上で動かしている場合
        if (Application.isEditor)
        {
            switch (m_settingManager.GetGravityController)
            {
                case ControllerState.Joystick:
                    //ステックテスト
                    m_vector3.x = m_joystick.Horizontal;
                    m_vector3.z = m_joystick.Vertical;
                    m_vector3.y = -1.0f;
                    break;
                case ControllerState.Acceleration:
                    //キー入力を検知ベクトルを設定
                    m_vector3.x = Input.GetAxis("Horizontal");
                    m_vector3.z = Input.GetAxis("Vertical");
                    m_vector3.y = -1.0f;
                    //テスト用
                    if (Input.GetKey("z"))
                    {
                        m_vector3.y = 0f;
                    }
                    break;
            }
        }
        //スマホデバッグ用
        else
        {
            switch (m_settingManager.GetGravityController)
            {
                case ControllerState.Joystick:
                    //キー入力を検知ベクトルを設定
                    m_vector3.x = m_joystick.Horizontal;
                    m_vector3.z = m_joystick.Vertical;
                    m_vector3.y = -1.0f;
                    break;
                case ControllerState.Acceleration:
                    //加速度センサーの入力をUnity空間の軸にマッピングする(座標軸が異なるため)
                    m_vector3.x = Input.acceleration.x;
                    m_vector3.z = Input.acceleration.y;
                    m_vector3.y = -1.0f;//マップ外に行かないようにする
                    break;
            }
        }
        //シーンの重力を入力ベクトルの方向に合わせて変化させる
        Physics.gravity = m_gravity * m_vector3.normalized * m_gravityScale;
        if (m_debug)
        {
            Debug.Log(Physics.gravity);
        }
    }

    /// <summary>
    /// 重力操作変更ボタン
    /// </summary>
    public void GravityControllerButton()
    {
        switch (m_settingManager.GetGravityController)
        {
            case ControllerState.Joystick:
                Acceleration();
                break;
            case ControllerState.Acceleration:
                Joystick();
                break;
        }
    }

    /// <summary>
    /// コントロールをジョイスティックにする
    /// </summary>
    public void Joystick()
    {
        m_joystickGameObject.SetActive(true);
        m_settingManager.ChangeGravityController(ControllerState.Joystick);
    }

    /// <summary>
    /// コントロールを加速度センサーにする
    /// </summary>
    public void Acceleration()
    {
        m_joystickGameObject.SetActive(false);
        m_settingManager.ChangeGravityController(ControllerState.Acceleration);
    }

    /// <summary>
    /// 操作種類
    /// </summary>
    public enum ControllerState
    {
        Joystick,
        Acceleration
    }
}