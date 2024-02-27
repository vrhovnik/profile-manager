using System.Security.Cryptography;
using System.Text;

namespace PM.Core;

public static class UserHelper
{
    public static string GetUniqueValue(this string input)
    {
        ArgumentException.ThrowIfNullOrEmpty(input);
            
        var shaHash = SHA256.Create();
        var data = shaHash.ComputeHash(Encoding.UTF8.GetBytes(input));
        var sBuilder = new StringBuilder();
        foreach (var currentByte in data) sBuilder.Append(currentByte.ToString("x2"));

        return sBuilder.ToString();
    }
}