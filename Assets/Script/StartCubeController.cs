using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// スタートキューブの当たり判定用
/// </summary>
public class StartCubeController : MonoBehaviour
{
    /// <summary>スタートキューブから出た際の処理</summary>
    [SerializeField] private UnityEvent m_startCubeEvent = new UnityEvent();
    /// <summary>ゲームマネージャー</summary>
    TimeManager m_timemanager;

    // Start is called before the first frame update
    void Start()
    {
        //ゲームマネージャーがあった場合タイムメソッドを入れる
        m_timemanager = GameObject.FindObjectOfType<TimeManager>();
        if (m_timemanager)
        {
            m_startCubeEvent.AddListener(m_timemanager.TimerStart);
        }
    }

    /// <summary>スタートキューブから出た時</summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        //プレイヤーがスタートから離れたら
        if (other.gameObject.tag == "Player")
        {
            m_startCubeEvent?.Invoke();
        }
    }
}