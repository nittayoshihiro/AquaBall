using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCubeController : MonoBehaviour
{
    GameManager m_gamemanager;
    // Start is called before the first frame update
    void Start()
    {
        m_gamemanager = GameObject.FindObjectOfType<GameManager>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            m_gamemanager.PlayerStart();
        }
    }
}
