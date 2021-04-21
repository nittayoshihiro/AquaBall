using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;//ファイルシステムを使用するため

/// <summary>
/// ファイルを管理するクラス
/// </summary>
public class FileController
{
    /// <summary>
    /// ファイルを作ります。
    /// </summary>
    /// <param name="Path"></param>
    public static void CreateFile(string textName)
    {
        Debug.Log($"ファイル '{GetFilePath(textName)}' を作ります");
        File.Create(GetFilePath(textName));

    }

    /// <summary>
    /// ファイルセーブ （上書き）
    /// </summary>
    /// <param name="textName">ファイル名前</param>
    public static void TextSave(string textName, string text)
    {
        //指定されたファイルが存在しない場合、このパラメーターは無効であり、コンストラクターは新しいファイルを作成します。
        using (var writer = new StreamWriter(GetFilePath(textName), append: false))
        {
            writer.Write(text);
        }
    }

    /// <summary>
    /// ファイル読み取る
    /// </summary>
    /// <param name="textName">ファイル名前</param>
    /// <returns>ファイルの中身</returns>
    public static string TextLoad(string textName)
    {
        string text = "";
        try
        {
            using (var reader = new StreamReader(GetFilePath(textName)))
            {
                while (!reader.EndOfStream)//テキストを行単位で読み込む　ReadToEndメソッドはまとめて読み込むのでメモリー消費が大きくなる　独習c＃p195 参照
                {
                    string line = reader.ReadLine();
                    Debug.Log(line);
                    //上書きの際元のもて見れるようにするため(消しても保存先は影響されない)
                    text += line;
                }

            }
        }
        catch (FileNotFoundException ex)
        {
            Debug.Log($"{ex}のファイルが見つかりませんでした。");
        }
        return text;
    }


    /// <summary>
    /// ファイルパスを返します。
    /// </summary>
    /// <returns>ファイルパス</returns>
    public static string GetFilePath(string textName)
    {
        // Unity の場合はどこでもファイルの読み書きができるわけではないことに注意。Application.persistentDataPath を使って「読み書きできるところ」でファイル操作をすること。
        string filePath = Application.persistentDataPath + "/" + (textName == "" ? Application.productName : textName) + ".json";
        return filePath;
    }
}