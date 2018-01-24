using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace UGFramework.Utility
{
    public static class MD5Utility
    {
        public static string GetFileMD5(string fullpath)
        {
            var bytes = FileUtility.ReadFileBytes(fullpath);
            return GetBytesMD5(bytes);
        }

        public static string GetBytesMD5(byte[] bytes)
        {
            var sb = new StringBuilder();
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(bytes);
                foreach (var b in hash)
                    sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
