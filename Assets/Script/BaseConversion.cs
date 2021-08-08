using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseConversion
{
    /// <summary>
    /// 32進数の配列
    /// </summary>
    private static char[] num32 = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'N', 'M', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V' };
    /// <summary>１ケタ</summary>
    char x1 = '0';
    /// <summary>２ケタ</summary>
    char x2 = '0';
    /// <summary>３ケタ</summary>
    char x3 = '0';
    /// <summary>４ケタ</summary>
    char x4 = '0';
    /// <summary>32進数の変換結果</summary>
    string xfin = default;
    /// <summary>32進数のそれぞれの桁の重み</summary>
    int xv1, xv2, xv3, xv4;


    //private static string[] num32 = new[]
    // {"0","1","2","3","4","5","6","7","8","9","A","B","C","D","E","F","G",
    //    "H","I","J","K","L","N","M","O","P","Q","R","S","T","U","V","10"};

    ///// <summary>
    ///// 32までの数字を32進数で返します
    ///// </summary>
    ///// <param name="num"></param>
    ///// <returns></returns>
    //public static string Conversion32(int num)
    //{
    //    Debug.Log(num32.Length);
    //    Debug.Log(num);
    //    if (num < 32)
    //    {
    //        string conversionnum = num32[num];
    //        return conversionnum;
    //    }
    //    else
    //    {
    //        Debug.Log("変換できます");
    //        return null;
    //    }
    //}

    /// <summary>
    /// 10進数を32進数で返します。
    /// </summary>
    /// <param name="num"></param>
    public string Convers32(int num)
    {
        xv1 = (int)(num / (Math.Pow(32, 0))) % 32;
        xv2 = (int)(num / (Math.Pow(32, 1))) % 32;
        xv3 = (int)(num / (Math.Pow(32, 2))) % 32;
        xv4 = (int)(num / (Math.Pow(32, 3))) % 32;
        for (int i = 0; i < num32.Length; i++)
        {
            if (xv1 == i)
            {
                x1 = num32[i];
            }
            if (xv2 == i)
            {
                x2 = num32[i];
            }
            if (xv3 == i)
            {
                x3 = num32[i];
            }
            if (xv4 == i)
            {
                x4 = num32[i];
            }

        }
        xfin = x4 + x3 + x2 + x1.ToString();
        return xfin;
        //string conversionnum = Conversion32(num % 32) + Conversion32(num % 1024 / 32);
        //return conversionnum;
    }

    /// <summary>
    /// 10進数の配列を32進数で返します
    /// </summary>
    /// <param name="nums"></param>
    /// <returns></returns>
    public string[] ColumnConvers32(int[] nums)
    {
        string[] conversionnum = new string[nums.Length];
        for (int i = 0; i < nums.Length - 2; i++)
        {
            conversionnum[i] = Convers32(nums[i]);//addかもしれない
        }
        conversionnum[conversionnum.Length - 1] = nums[nums.Length - 1].ToString();
        return conversionnum;
    }

    /// <summary>
    /// 32進数の配列を10進数で返します。
    /// </summary>
    /// <param name="nums"></param>
    /// <returns></returns>
    public static int[] ColumnConvert10(string[] nums)
    {
        int[] conversionnum = new int[nums.Length];
        //for (int i = 0; i < nums.Length - 1; i++)
        //{
        //    conversionnum[i] = Convert10(nums[i]);
        //    Debug.Log("nums:" + nums[i] + "coversionnumi:" + conversionnum[i]);
        //}
        //conversionnum[conversionnum.Length - 1] = int.Parse(nums[nums.Length - 1]);
        return conversionnum;
    }

    ///// <summary>
    ///// 32進数を10進数で返します。
    ///// </summary>
    ///// <param name="num"></param>
    ///// <returns></returns>
    //public static int Convert10(string num)
    //{
    //    int conversionnum = default;

    //    for (int i = 0; i < num32.Length; i++)
    //    {
    //        //1ケタ
    //        if (1 <= num.Length && num[num.Length - 1].ToString() == num32[i])
    //        {
    //            conversionnum = (i + 1) * 1;
    //        }
    //        //2ケタ
    //        if (2 <= num.Length && num[num.Length - 2].ToString() == num32[i])
    //        {
    //            conversionnum = (i + 1) * 10;
    //        }
    //        //3ケタ
    //        if (3 <= num.Length && num[num.Length - 3].ToString() == num32[i])
    //        {
    //            conversionnum = (i + 1) * 10;
    //        }
    //    }
    //    return conversionnum;
    //}
}
