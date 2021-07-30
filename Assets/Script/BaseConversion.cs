using System.Collections;
using System.Collections.Generic;

public class BaseConversion
{
    private static string[] num32 = new[]
     {"1","2","3","4","5","6","7","8","9","A","B","C","D","E","F","G",
        "H","I","J","K","L","N","M","O","P","Q","R","S","T","U","V"};

    /// <summary>
    /// 32までの数字を32進数で返します
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static string Conversion32(int num)
    {
        string conversionnum;
        conversionnum = num32[num - 1];
        return conversionnum;
    }

    /// <summary>
    /// 10進数を32進数で返します。
    /// </summary>
    /// <param name="num"></param>
    public void FullConvers32(int num)
    {
        string conversionnum;
        int onesPlace;
        onesPlace = num % 32;
        int tensPlace;
        tensPlace = num / 32;
        
        
    }
}
