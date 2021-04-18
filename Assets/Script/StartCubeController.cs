using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スタートキューブの当たり判定用
/// </summary>
public class StartCubeController : MonoBehaviour
{
    /// <summary>スタートキューブから出た際のメソッド型</summary>
    delegate void StartCubeExitEvent();
    /// <summary>スタートキューブから出た際のメソッド</summary>
    private StartCubeExitEvent m_startCubeExitEvent;
    /// <summary>ゲームマネージャー</summary>
    TimeManager m_timemanager;

    // Start is called before the first frame update
    void Start()
    {
        //ゲームマネージャーがあった場合タイムメソッドを入れる
        m_timemanager = GameObject.FindObjectOfType<TimeManager>();
        if (m_timemanager)
        {
            m_startCubeExitEvent = new StartCubeExitEvent(m_timemanager.TimerStart);
        }
    }

    /// <summary>スタートキューブから出た時</summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        //プレイヤーがスタートから離れたら
        if (other.gameObject.tag == "Player")
        {
            //メソッドがある時呼び出す
            m_startCubeExitEvent?.Invoke();
        }
    }
}