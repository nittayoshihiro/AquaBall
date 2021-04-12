using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スタートキューブの当たり判定用
/// </summary>
public class StartCubeController : MonoBehaviour
{
    GameManager m_gamemanager;
    // Start is called before the first frame update
    void Start()
    {
        m_gamemanager = GameObject.FindObjectOfType<GameManager>();
    }

    /// <summary>プレイヤーがスタートから離れたら</summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            m_gamemanager.TimerStart();
        }
    }
}