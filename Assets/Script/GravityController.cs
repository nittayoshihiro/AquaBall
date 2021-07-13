using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    [SerializeField] GameObject m_joystickGameObject = null;
    /// <summary>コントローラー表示テキスト</summary>
    [SerializeField] Text m_textController = null;
    /// <summary>FloatJoystick</summary>
    FloatingJoystick m_joystick = null;
    /// <summary>重力規模</summary>
    [SerializeField] float m_gravityScale = 1.0f;
    /// <summary>デバッグonoff</summary>
    [SerializeField] bool m_debug = true;
    /// <summary>加速度の補強</summary>
    [SerializeField] private float m_strongAcceleration = 2.0f;//２倍にします
    /// <summary>設定</summary>
    SettingManager m_settingManager = null;
    GameManager m_gameManager = null;
    /// <summary>ゲーム状態</summary>
    GameManagerBaseState m_gameState;
    const float m_gravityScaleY = -2.0f;
    /// <summary>ジョイスティックステート</summary>
    StateJoystick m_stateJoystick = new StateJoystick();
    /// <summary>加速度センサーステート</summary>
    StateAcceleration m_stateAcceleration = new StateAcceleration();
    /// <summary>現在のステート</summary>
    GravityControllerBaseState m_currentControllerState;

    private void Start()
    {
        m_gameManager = GetComponent<GameManager>();
        m_joystick = m_joystickGameObject.GetComponent<FloatingJoystick>();
        m_settingManager = FindObjectOfType<SettingManager>();
        if (m_debug)
        {
            Debug.Log(m_settingManager);
        }

        m_stateJoystick.SetStateId(StateId.JoyStick);
        m_stateAcceleration.SetStateId(StateId.Acceleration);
        m_settingManager.SetUp();
        SetUPState();
    }

    /// <summary>
    /// ステートのセットアップ
    /// </summary>
    public void SetUPState()
    {
        if (m_settingManager.GetGravityController == StateId.JoyStick)
        {
            m_currentControllerState = m_stateJoystick;
        }
        else if (m_settingManager.GetGravityController == StateId.Acceleration)
        {
            m_currentControllerState = m_stateAcceleration;
        }
    }

    /// <summary>
    /// マイフレーム呼ばれるメソッド
    /// </summary>
    public void OnUpdate()
    {
        m_currentControllerState.OnUpdate(this);
    }

    /// <summary>
    /// 重力操作変更ボタン
    /// </summary>
    public void GravityControllerButton()
    {
        switch (m_settingManager.GetGravityController)
        {
            case StateId.JoyStick:
                ChangeState(m_stateAcceleration);
                m_settingManager.ChangeGravityController(m_stateAcceleration);
                break;
            case StateId.Acceleration:
                ChangeState(m_stateJoystick);
                m_settingManager.ChangeGravityController(m_stateJoystick);
                break;
            case StateId.None:
                break;
        }
        ControllerText();
    }

    /// <summary>
    /// 設定されている状態を表示する
    /// </summary>
    public void ControllerText()
    {
        switch (m_currentControllerState.stateId)
        {
            case StateId.JoyStick:
                m_textController.text = "Joystick";
                break;
            case StateId.Acceleration:
                m_textController.text = "Acceleration";
                break;
            case StateId.None:
                break;
        }
    }

    /// <summary>
    /// ジョイスティックを可能にするか判定
    /// </summary>
    public void JoystickJudgment()
    {
        m_gameState = m_gameManager.GetGameState;
        Debug.Log("スティックジャッジ:"+m_gameManager.GetGameState.gameManagerId);
        if (m_gameManager.GetGameState.gameManagerId == GameManagerId.InGame && m_currentControllerState.stateId == StateId.JoyStick)
        {
            m_joystickGameObject.SetActive(true);
        }
        else
        {
            PointerEventData eventData = default;
            m_joystick.OnPointerUp(eventData);
            m_joystickGameObject.SetActive(false);
        }
    }

    /// <summary>
    /// コントロールをジョイスティックにする
    /// </summary>
    public void Joystick()
    {
        ChangeState(m_stateJoystick);
        JoystickJudgment();
        ControllerText();
    }

    /// <summary>
    /// コントロールを加速度センサーにする
    /// </summary>
    public void Acceleration()
    {
        PointerEventData eventData = new PointerEventData(FindObjectOfType<EventSystem>().GetComponent<EventSystem>());
        Debug.Log(eventData);
        if (eventData != null)
        {
            m_joystick.OnPointerUp(eventData);
        }

        m_joystickGameObject.SetActive(false);
        ChangeState(m_stateAcceleration);
        ControllerText();
    }

    /// <summary>
    /// 重力初期化
    /// </summary>
    public void ResetGravity()
    {
        Physics.gravity = m_gravity * new Vector3(0, 1, 0).normalized * m_gravityScale;
    }

    /// <summary>
    /// ステートを変えます
    /// </summary>
    public void ChangeState(GravityControllerBaseState nextBaseState)
    {
        m_currentControllerState.OnExit(this, nextBaseState);
        nextBaseState.OnEnter(this, m_currentControllerState);
        m_currentControllerState = nextBaseState;
    }

    /*public T GetCurrentState<T>() where T : GravityControllerBaseState
        => m_currentState as T;*/

    public StateId GetCurrentState => m_currentControllerState.stateId;

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
#if UNITY_EDITOR||UNITY_STANDALONE_WIN
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