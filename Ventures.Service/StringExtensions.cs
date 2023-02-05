using System.Text;

namespace Ventures.Service;

public static class StringExtensions
{
    public static string ToAscii(this byte[] bytes)
    {
        return Encoding.ASCII.GetString(bytes, 0, bytes.Length);
    }

    public static byte[] ToBytes(this string from)
    {
        return Encoding.ASCII.GetBytes(from);
    }
    
    public static string Reversed(this string input)
    {
        var charArray = input.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);;
    }
}