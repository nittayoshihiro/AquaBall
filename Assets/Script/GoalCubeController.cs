using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゴールキューブの当たり判定用
/// </summary>
public class GoalCubeController : MonoBehaviour
{
    GameManager m_gamemanager;
    // Start is called before the first frame update
    void Start()
    {
        m_gamemanager = GameObject.FindObjectOfType<GameManager>();
    }

    /// <summary>プレイヤーがスタートから離れたら</summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            m_gamemanager.EndOfGame();
        }
    }
}
