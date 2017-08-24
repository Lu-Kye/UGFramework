using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using UGFramework.Extension;

namespace UGFramework.Utility
{
    public static class FileUtility
    {
        public static byte[] ReadFileBytes(string fullpath)
        {
            byte[] bytes;
            using (var fs = File.Open(fullpath, FileMode.Open))
            {
                bytes = new byte[fs.Length];
                fs.Read(bytes, 0, (int)fs.Length);
            }
            return bytes;
        }
    
        public static string ReadFile(string fullpath)
        {
            var bytes = ReadFileBytes(fullpath);
            if (bytes == null)
                return null;
            return Encoding.UTF8.GetString(bytes);
        }
    
        public static void WriteFile(string fullpath, string text, bool append = false) 
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            WriteFile(fullpath, bytes, bytes.Length, append);
        }
    
        public static void WriteFile(string fullpath, byte[] bytes, int bytesLength, bool append = false)
        {
            var fileInfo = new FileInfo(fullpath);
            if (fileInfo.Exists == false)
                fileInfo.Directory.Create();
    
            if (append == false && fileInfo.Exists == true)
            {
                using (var fs = fileInfo.Open(FileMode.Truncate, FileAccess.Write))
                {
                    fs.Write(bytes, 0, bytesLength);
                }
            }
            else
            {
                using (var fs = fileInfo.Open(FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.Write(bytes, 0, bytesLength);
                }
            }
        }
    
        public static List<string> ReadCSV(string filePath)
        {
            var dt = new List<string>();
            Encoding encoding = Encoding.UTF8;
            FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs, encoding);
            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine = null;
            string[] tableHead = null;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool IsFirst = true;
    
            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                if (IsFirst == true)
                {
                    tableHead = strLine.Split(',');
                    IsFirst = false;
                    columnCount = tableHead.Length;
    
                    var line = "";
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        line += tableHead[i];
                        line += ",";
                    }
                    dt.Add(line);
                }
                else
                {
                    aryLine = strLine.Split(',');
                    var line = "";
                    for (int j = 0; j < columnCount; j++)
                    {
                        line += j >= aryLine.Length ? "" : aryLine[j];
                        line += ",";
                    }
                    dt.Add(line);
                }
            }
            sr.Close();
            fs.Close();
            return dt;
        }
    
        public static void CompressFile(string sourcePath, string targetPath)
        {
            // Read file into byte array buffer.
            byte[] b;
            using (FileStream f = new FileStream(sourcePath, FileMode.Open))
            {
                b = new byte[f.Length];
                f.Read(b, 0, (int)f.Length);
            }
    
            // Use GZipStream to write compressed bytes to target file.
            using (FileStream f2 = new FileStream(targetPath, FileMode.OpenOrCreate))
            {
                using (GZipStream gz = new GZipStream(f2, CompressionMode.Compress, false))
                {
                    gz.Write(b, 0, b.Length);
                }
            }
        }
    
        public static void DeCompressFile(string sourcePath, string targetPath)
        {
            // Read file & decompress into byte array buffer.
            byte[] b = new byte[0];
            using (Stream f = File.Open(sourcePath, FileMode.Open))
            {
                using (Stream gz = new GZipStream(f, CompressionMode.Decompress))
                {
                    // 5mb
                    byte[] rb = new byte[5*1024*1024];
                    int n;
                    while ((n = gz.Read(rb, 0, rb.Length)) > 0)
                    {
                        b = b.Merge(rb, n);
                    }
                }
            }
    
            // Use FileStream to write bytes to target file.
            WriteFile(targetPath, b, b.Length, false);
        }
    
        public static void DeCompressBuffer(byte[] bytes, int bytesLength, string targetPath)
        {
            // Read buffer & decompress into byte array buffer.
            byte[] b = new byte[0];
            var buffer = new byte[bytesLength];
            Array.Copy(bytes, 0, buffer, 0, bytesLength);
            using (MemoryStream f = new MemoryStream(buffer))
            {
                using (Stream gz = new GZipStream(f, CompressionMode.Decompress))
                {
                    // 5mb
                    byte[] rb = new byte[5*1024*1024];
                    int n;
                    while ((n = gz.Read(rb, 0, rb.Length)) > 0)
                    {
                        b = b.Merge(rb, n);
                    }
                }
            }
    
            // Use FileStream to write bytes to target file.
            WriteFile(targetPath, b, b.Length, false);
        }
    }
}