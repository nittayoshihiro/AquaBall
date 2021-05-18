using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ResultDataController : MonoBehaviour
{
    /// <summary>データ名前</summary>
    public const string m_textName = "ResultData";
    /// <summary>リザルトデータ</summary>
    ResultData m_resultData = null;
    /// <summary>ランキングテキスト</summary>
    [SerializeField] Text m_rankingText = null;
    /// <summary>プレイヤーの順位</summary>
    private int m_playerRankig = 0;//0:順位外　1:１位　2:２位　3:３位
    /// <summary>結果を更新するか</summary>
    private bool m_update = true;
    /// <summary>セーブボタン</summary>
    [SerializeField] GameObject m_saveButton = null;

    /// <summary>
    ///データ初期化する前
    /// </summary>
    [RuntimeInitializeOnLoadMethod()]
    static void BeforInit()
    {
        //データがあったら
        if (!File.Exists(FileController.GetFilePath(m_textName)))
        {
            Debug.Log($"{FileController.GetFilePath(m_textName)}のファイルが見つかりませんでした。ファイルを作ります");
            ResultData resultData = new ResultData(999, 999, 999);
            FileController.TextSave(m_textName, JsonUtility.ToJson(resultData));
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_resultData = GetResultLoad;
    }

    /// <summary>
    /// セーブボタンを表示非表示
    /// </summary>
    /// <param name="active">アクティブ</param>
    public void SaveButtonActive(bool active)
    {
        m_saveButton.SetActive(active);
    }

    /// <summary>
    /// 結果をセーブする
    /// </summary>
    public void ResultSave()
    {
        ResultSave(m_resultData);
    }

    /// <summary>
    /// セーブされてるデータを取得、結果を更新オン
    /// </summary>
    public void ResautLoad()
    {
        m_update = true;
        m_resultData = GetResultLoad;
    }

    /// <summary>
    /// 結果をリセット、結果を更新オフ
    /// </summary>
    public void ResultDelet()
    {
        m_update = false;
        m_resultData = new ResultData(999, 999, 999);
    }

    /// <summary>
    /// ランキングテキストを更新表示します
    /// </summary>
    public void RankingText(float resultTime)
    {
        RankingReload(resultTime, m_update);
        m_rankingText.text = RankingFormat + "\n  <size=70>Time:" + string.Format("{0:000.00}", resultTime) + "</size>";
    }

    /// <summary>
    /// ランキングテキスト形式を返します。(順位によって色が変わります)
    /// </summary>
    private string RankingFormat
    {
        get
        {
            string text = null;
            switch (m_playerRankig)
            {
                case 0:
                    text = "1 <size=50>st</size>" + string.Format("{0:000.00}", m_resultData.firstPlace)
               + "\n2<size=50>nd</size>" + string.Format("{0:000.00}", m_resultData.secondPlace)
               + "\n3<size=50>rd</size>" + string.Format("{0:000.00}", m_resultData.thirdPlace);
                    break;
                case 1:
                    text = "<color=#ffff00>1 <size=50>st</size>" + string.Format("{0:000.00}", m_resultData.firstPlace) + "</color>"
               + "\n2<size=50>nd</size>" + string.Format("{0:000.00}", m_resultData.secondPlace)
               + "\n3<size=50>rd</size>" + string.Format("{0:000.00}", m_resultData.thirdPlace);
                    break;
                case 2:
                    text = "1 <size=50>st</size>" + string.Format("{0:000.00}", m_resultData.firstPlace)
               + "\n<color=#ffff00>2<size=50>nd</size>" + string.Format("{0:000.00}", m_resultData.secondPlace) + "</color>"
               + "\n3<size=50>rd</size>" + string.Format("{0:000.00}", m_resultData.thirdPlace);
                    break;
                case 3:
                    text = "1 <size=50>st</size>" + string.Format("{0:000.00}", m_resultData.firstPlace)
               + "\n2<size=50>nd</size>" + string.Format("{0:000.00}", m_resultData.secondPlace)
               + "\n<color=#ffff00>3<size=50>rd</size>" + string.Format("{0:000.00}", m_resultData.thirdPlace) + "</color>";
                    break;
            }
            return text;
        }
    }

    /// <summary>
    /// ランキングを変更する
    /// </summary>
    /// <param name="resultTime">結果タイム</param>
    /// <param name="update">更新するか</param>
    private void RankingReload(float resultTime, bool update)
    {
        if (resultTime <= m_resultData.firstPlace)
        {
            m_resultData.thirdPlace = m_resultData.secondPlace;
            m_resultData.secondPlace = m_resultData.firstPlace;
            m_resultData.firstPlace = resultTime;
            if (update)
            {
                ResultSave(m_resultData);
            }
            m_playerRankig = 1;
        }
        else if (resultTime <= m_resultData.secondPlace)
        {
            m_resultData.thirdPlace = m_resultData.secondPlace;
            m_resultData.secondPlace = resultTime;
            if (update)
            {
                ResultSave(m_resultData);
            }
            m_playerRankig = 2;
        }
        else if (resultTime <= m_resultData.thirdPlace)
        {
            m_resultData.thirdPlace = resultTime;
            if (update)
            {
                ResultSave(m_resultData);
            }
            m_playerRankig = 3;
        }
        else
        {
            m_playerRankig = 0;
        }
    }

    /// <summary>
    /// 結果セーブ
    /// </summary>
    /// <param name="resultData">結果データ</param>
    public void ResultSave(ResultData resultData)
    {
        FileController.TextSave(m_textName, JsonUtility.ToJson(resultData));
    }

    /// <summary>
    /// 結果ロードして返す
    /// </summary>
    private ResultData GetResultLoad
    {
        get
        {
            ResultData resultData = JsonUtility.FromJson<ResultData>(FileController.TextLoad(m_textName));
            return resultData;
        }
    }
}

/// <summary>
/// 結果データ(Json)形式
/// </summary>
[Serializable]
public class ResultData
{
    /// <summary>１位</summary>
    public float firstPlace;
    /// <summary>２位</summary>
    public float secondPlace;
    /// <summary>３位</summary>
    public float thirdPlace;

    /// <summary>
    /// 順位初期化
    /// </summary>
    /// <param name="first">１位</param>
    /// <param name="second">２位</param>
    /// <param name="third">３位</param>
    public ResultData(float first, float second, float third)
    {
        firstPlace = first;
        secondPlace = second;
        thirdPlace = third;
    }
}