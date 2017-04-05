using System.IO;
using System.Text;

namespace UGFramework.Utility
{
    public static class FileUtility
    {
        public static byte[] ReadFileBytes(string fullpath)
        {
            byte[] bytes;
            using (var fs = File.Open(fullpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                bytes = new byte[fs.Length];
                fs.Read(bytes, 0, (int)fs.Length);
            }
            return bytes;
        }

        public static void WriteFile(string fullpath, string text, bool append = false) 
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            WriteFile(fullpath, bytes, append);
        }

        public static void WriteFile(string fullpath, byte[] bytes, bool append = false)
        {
            var directory = Path.GetDirectoryName(fullpath);
            if (Directory.Exists(directory) == false)
                Directory.CreateDirectory(directory);
            if (append == false)
            {
                using (var fs = File.Open(fullpath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            else
            {
                if (File.Exists(fullpath) == false)
                    using (var fs = File.Create(fullpath)) {}

                using (var fs = File.Open(fullpath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
        }
    }
}