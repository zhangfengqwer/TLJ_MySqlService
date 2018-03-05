using System;
using System.Text;

public class RandomCharHelper
{
    private static char[] allcheckRandom =
    {
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V',
        'W',
        'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q',
        'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
    };

    private static char[] allNumRandom =
    {
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
    };

    public static string GetRandomNum()
    {
        int i = new Random(GetRandomSeed()).Next(0, allNumRandom.Length);

        return allNumRandom[i].ToString();
    }

    public static string GetRandomNum(int length)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            string randomNum = GetRandomNum();
            sb.Append(randomNum);
        }

        return sb.ToString();
    }

    public static string GetRandomChar(int length)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            string randomChar = GetRandomChar();
            sb.Append(randomChar);
        }

        return sb.ToString();
    }

    public static string GetRandomChar()
    {
        int i = new Random(GetRandomSeed()).Next(0, allcheckRandom.Length);

        return allcheckRandom[i].ToString();
    }

    public static int GetRandomSeed()
    {
        byte[] bytes = new byte[4];
        System.Security.Cryptography.RNGCryptoServiceProvider rng =
            new System.Security.Cryptography.RNGCryptoServiceProvider();
        rng.GetBytes(bytes);
        return BitConverter.ToInt32(bytes, 0);
    }
}