using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ボタンシステム
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonSystem : MonoBehaviour
{
    [Header("クリック時に流すSE"),SerializeField] AudioClip m_audioClip = null;
    [SerializeField]AudioSource m_audioSource= null;
    Button m_button = null;

    // Start is called before the first frame update
    public virtual void Start()
    {
        m_button = GetComponent<Button>();
        if (m_button)
        {
            m_button.onClick.AddListener(OnClick);
        }
    }

    /// <summary>
    /// 動的にボタン（onClick）に機能を追加する
    /// </summary>
    public virtual void OnClick()
    {
        SoundPlay();
    }

    /// <summary>音を鳴らします</summary>
    private void SoundPlay()
    {
        m_audioSource.PlayOneShot(m_audioClip);
    }
}
