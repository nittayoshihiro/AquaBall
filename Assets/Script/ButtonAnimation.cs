using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// ボタンのアニメーションスクリプト
/// </summary>
public class ButtonAnimation : ButtonSystem
{
    RectTransform m_rectTransform = null;
    EventTrigger m_eventTrigger = null;
    [SerializeField] float m_min = 0.9f;
    [SerializeField] float m_max = 1f;
    /// <summary>ポイントダウンイベントがあるか</summary>
    bool m_pointerDownChack = false;
    /// <summary>ポイントイグジットイベントがあるか</summary>
    bool m_pointerExtitChack = false;


    public override void Start()
    {
        base.Start();

        m_rectTransform = GetComponent<RectTransform>();
        m_eventTrigger = GetComponent<EventTrigger>();

        foreach (var item in m_eventTrigger.triggers)
        {
            if (item.eventID == EventTriggerType.PointerDown)
            {
                item.callback.AddListener(ButtonEfectMin);
                m_pointerDownChack = true;
            }
            if (item.eventID == EventTriggerType.PointerExit)
            {
                item.callback.AddListener(ButtonEfectMax);
                m_pointerExtitChack = true;
            }
        }


        //ポイントダウンイベントがない場合作成しメソッドを追加する
        if (!m_pointerDownChack)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;

            entry.callback.AddListener((x) => ButtonEfectMin(x));
            m_eventTrigger.triggers.Add(entry);
        }
        //ポイントイグジットがない場合作成しメソッドを追加する
        if (!m_pointerExtitChack)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerExit;

            entry.callback.AddListener((x) => ButtonEfectMax(x));
            m_eventTrigger.triggers.Add(entry);
        }
    }

    
    public override void OnClick()
    {
        base.OnClick();
    }

    /// <summary>ボタン効果最小</summary>
    private void ButtonEfectMin(BaseEventData baseEventData)
    {
        m_rectTransform.transform.localScale = new Vector3(m_min, m_min, 0);
    }

    /// <summary>ボタン効果最大</summary>
    private void ButtonEfectMax(BaseEventData baseEventData)
    {
        m_rectTransform.transform.localScale = new Vector3(m_max, m_max, 0);
    }

}
