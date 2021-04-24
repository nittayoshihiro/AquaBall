using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ResultDataController : MonoBehaviour
{
    /// <summary>データ名前</summary>
    static string m_textName = "ResultData";
    /// <summary>リザルトデータ</summary>
    ResultData m_resultData = null;
    /// <summary>ランキングテキスト</summary>
    [SerializeField] Text m_rankingText = null;

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
            ResultData resultData = new ResultData(0, 0, 0);
            FileController.TextSave(m_textName, JsonUtility.ToJson(resultData));
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_resultData = GetResultLoad;
    }

    /// <summary>
    /// ランキングテキストを更新表示します
    /// </summary>
    public void RankingText(float resultTime)
    {
        RankingReload(resultTime);
        m_rankingText.text = RankingFormat;
    }

    /// <summary>
    /// ランキングテキスト形式を返します。
    /// </summary>
    private string RankingFormat
    {
        get
        {
            string text = "１位" + m_resultData.firstPlace + "\n２位" + m_resultData.secondPlace + "\n３位" + m_resultData.thirdPlace;
            return text;
        }
    }

    /// <summary>
    /// ランキングを変更する
    /// </summary>
    /// /// <param name="resultTime">結果タイム</param>
    private void RankingReload(float resultTime)
    {
        if (resultTime >= m_resultData.firstPlace)
        {
            m_resultData.thirdPlace = m_resultData.secondPlace;
            m_resultData.secondPlace = m_resultData.firstPlace;
            m_resultData.firstPlace = resultTime;
            ResultSave(m_resultData);
        }
        else if (resultTime >= m_resultData.secondPlace)
        {
            m_resultData.thirdPlace = m_resultData.secondPlace;
            m_resultData.secondPlace = resultTime;
            ResultSave(m_resultData);
        }
        else if (resultTime >= m_resultData.thirdPlace)
        {
            m_resultData.thirdPlace = resultTime;
            ResultSave(m_resultData);
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
    public ResultData GetResultLoad
    {
        get
        {
            ResultData resultData = JsonUtility.FromJson<ResultData>(FileController.TextLoad(m_textName));
            Debug.Log(resultData);
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
    public ResultData(float first, float second, float third)
    {
        firstPlace = first;
        secondPlace = second;
        thirdPlace = third;
    }
}