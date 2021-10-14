using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ポップアップウィンドウクラス
/// </summary>
public class Popup : MonoBehaviour
{
    [SerializeField] Text titleText;
    [SerializeField] Text bodyText;
    [SerializeField] Button okButton;
    [SerializeField] Button cancelButton;
    [SerializeField] Button closeButton;

    public virtual void Initialize(string title, string body, Action okCallback, Action cancelcallback, Action closeCallback)
    {
        titleText.text = title;
        bodyText.text = body;

        okButton.onClick.AddListener(() => okCallback?.Invoke());
        cancelButton.onClick.AddListener(() => cancelcallback?.Invoke());
        closeButton.onClick.AddListener(() => closeCallback?.Invoke());
    }

}
