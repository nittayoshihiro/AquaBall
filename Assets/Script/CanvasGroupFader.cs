using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasGroupFader : MonoBehaviour
{
    /// <summary>アニメーションカーブ</summary>
    [SerializeField] AnimationCurve m_curve;/**< Tweenのイージングのカーブ*/
    [SerializeField] int m_inSpeed = 10;
    [SerializeField] int m_outSpeed = 5;
    /// <summary>
    /// 
    /// </summary>
    CanvasGroup m_canvasGroup = null;

    // Start is called before the first frame update
    void Start()
    {
        m_canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// フェードイン
    /// </summary>
    public void FadeIn()
    {
        StartCoroutine("FadeInCor");
    }

    /// <summary>
    /// フェードアウト
    /// </summary>
    public void FadeOut()
    {
        StartCoroutine("FadeOutCor");
    }

    /// <summary>
    /// フェイドインコルーチン
    /// </summary>
    IEnumerator FadeInCor()
    {
        while (true)
        {
            yield return null; // 1フレーム待つ
            m_canvasGroup.alpha += m_curve.Evaluate(Time.deltaTime * m_inSpeed);

            if (1f <= m_canvasGroup.alpha) // 不透明度が0以下のとき
            {
                m_canvasGroup.alpha = 1;// 不透明度を0
                break; // 繰り返し終了
            }
        }
    }

    /// <summary>
    /// フェイドアウトコルーチン
    /// </summary>
    IEnumerator FadeOutCor()
    {
        while (true)
        {
            yield return null; // 1フレーム待つ
            m_canvasGroup.alpha -= m_curve.Evaluate(Time.deltaTime * m_outSpeed);

            if (m_canvasGroup.alpha <= 0f) // 不透明度が0以下のとき
            {
                m_canvasGroup.alpha = 0;// 不透明度を0
                break; // 繰り返し終了
            }
            else if (!this.gameObject.activeSelf)
            {

                break; // 繰り返し終了
            }
        }
    }

    /// <summary>
    /// アルファ値を０にします。
    /// </summary>
    public void AlphaToZero()
    {
        m_canvasGroup.alpha = 0;// 不透明度を0
    }
}