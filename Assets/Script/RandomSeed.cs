using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSeed
{
    /// <summary></summary>
    private Random.State m_state;

    /// <summary>
    ///　インスタンス時にrandomのシード値を設定する
    /// </summary>
    /// <param name="seed"></param>
    public RandomSeed(int seed)
    {
        SetSeed(seed);
    }

    /// <summary>
    /// シード値をセットします
    /// </summary>
    /// <param name="seed"></param>
    public void SetSeed(int seed)
    {
        var prevstate = Random.state;
        Random.InitState(seed);
        m_state = Random.state;
        Random.state = prevstate;
    }

   /// <summary>
   /// int型のminからmaxまでの範囲内で返します
   /// </summary>
   /// <param name="min"></param>
   /// <param name="max"></param>
   /// <returns></returns>
    public int Range(int min, int max)
    {
        var prevstate = Random.state;
        Random.state = m_state;
        var result = Random.Range(min, max);
        m_state = Random.state;
        Random.state = prevstate;
        return result;
    }

    /// <summary>
    /// float型のminからmaxまでの範囲内で返します
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public float Range(float min, float max)
    {
        var prevstate = Random.state;
        Random.state = m_state;
        var result = Random.Range(min, max);
        m_state = Random.state;
        Random.state = prevstate;
        return result;
    }
}
