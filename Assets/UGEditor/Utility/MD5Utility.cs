using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class MD5Utility
{
    public static string GetFileMD5(string fullpath)
    {
        var sb = new StringBuilder();
        using (var md5 = MD5.Create())
        {
            using (var fs = new FileStream(fullpath, FileMode.Open))
            {
                int fsLen = (int)fs.Length;
                var bytes = new byte[fsLen];
                var hash = md5.ComputeHash(bytes);
                foreach (var b in hash)
                    sb.Append(b.ToString("x2"));
            }
        }
        return sb.ToString();
    }
}