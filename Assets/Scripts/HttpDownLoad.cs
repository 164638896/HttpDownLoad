using System.Threading;
using System.IO;
using System.Net;
using System;
using ICSharpCode.SharpZipLib.Zip;

public class HttpDownLoad
{
    public class ThreadParam
    {
        public int m_nID = 0;
        public string m_strURL = "";
        public string m_strSavePath = "";
        public Action<ThreadParam> callBack;
    }


    public float progress { get; private set; }
    //涉及子线程要注意,Unity关闭的时候子线程不会关闭，所以要有一个标识
    private bool isStop;
    private Thread thread;
    public bool isDone { get; private set; }

    /// <summary>
    /// 下载方法(断点续传)
    /// </summary>
    /// <param name="url">URL下载地址</param>
    /// <param name="savePath">Save path保存路径</param>
    /// <param name="callBack">Call back回调函数</param>
    public void DownLoad(object param)
    {
        ThreadParam tp = param as ThreadParam;

        isStop = false;
        if (!Directory.Exists(tp.m_strSavePath))
        {
            Directory.CreateDirectory(tp.m_strSavePath);
        }

        //获取下载文件的总长度
        long totalLength = GetLength(tp.m_strURL);
        if (totalLength > 0)
        {
            try
            {
                //这是要下载的文件路径
                string filePath = tp.m_strSavePath + "/" + tp.m_strURL.Substring(tp.m_strURL.LastIndexOf('/') + 1);

                //使用流操作文件
                FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                long fileLength = fs.Length;

                //如果没下载完
                if (fileLength < totalLength)
                {
                    //断点续传核心，设置本地文件流的起始位置
                    fs.Seek(fileLength, SeekOrigin.Begin);

                    HttpWebRequest request = HttpWebRequest.Create(tp.m_strURL) as HttpWebRequest;

                    request.Timeout = 5000;
                    //断点续传核心，设置远程访问文件流的起始位置
                    request.AddRange((int)fileLength);
                    Stream stream = request.GetResponse().GetResponseStream();

                    byte[] buffer = new byte[40960];
                    //使用流读取内容到buffer中
                    //注意方法返回值代表读取的实际长度,并不是buffer有多大，stream就会读进去多少
                    int length = stream.Read(buffer, 0, buffer.Length);
                    while (length > 0)
                    {
                        //UnityEngine.Debug.Log("length:" + length);

                        //如果Unity客户端关闭，停止下载
                        if (isStop) break;
                        //将内容再写入本地文件中
                        fs.Write(buffer, 0, length);
                        //计算进度
                        fileLength += length;
                        progress = (float)fileLength / (float)totalLength;
                        //UnityEngine.Debug.Log(progress);
                        fs.Flush();
                        //类似尾递归
                        length = stream.Read(buffer, 0, buffer.Length);
                    }

                    stream.Close();
                    stream.Dispose();
                }
                else
                {
                    progress = fileLength / totalLength;
                }

                fs.Close();
                fs.Dispose();

                //如果下载完毕，执行回调
                if (progress >= 1)
                {
                    isDone = true;
                    if (tp.callBack != null) tp.callBack(tp);

                    if (progress > 1)
                    {
                        UnityEngine.Debug.Log("下载错误:" + progress * 100 + "%");
                    }
                }

                UnityEngine.Debug.Log("下载进度为:" + progress * 100 + "%");
            }
            catch (WebException ex)
            {
                UnityEngine.Debug.Log("WebException Error code: " + ex.Status);

                //if (callBack != null) callBack();
            }
        }
        else
        {
            //if (callBack != null) callBack();
        }
    }

    /// <summary>
    /// 获取下载文件的大小
    /// </summary>
    /// <returns>The length.</returns>
    /// <param name="url">URL.</param>
    long GetLength(string url)
    {
        UnityEngine.Debug.Log(url);
        try
        {
            HttpWebRequest requet = HttpWebRequest.Create(url) as HttpWebRequest;
            requet.Method = "HEAD";
            requet.Timeout = 5000;
            //requet.ReadWriteTimeout = 5000;
            HttpWebResponse response = requet.GetResponse() as HttpWebResponse;
            return response.ContentLength;

        }
        catch (WebException ex)
        {
            UnityEngine.Debug.Log("WebException Error code: " + ex.Status);
        }

        return 0;
    }

    public void Close()
    {
        isStop = true;
    }

    public static void UnZipFile(string zipfile, string outPath)
    {
        // Perform simple parameter checking.
        //if (file.Length < 1)
        //{
        //    Console.WriteLine("Usage UnzipFile NameOfFile");
        //    return;
        //}

        if (!File.Exists(zipfile))
        {
            Console.WriteLine("Cannot find file '{0}'", zipfile);
            return;
        }

        try
        {

            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipfile)))
            {

                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {

                    Console.WriteLine(theEntry.Name);

                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);

                    // create directory
                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(outPath + "/" + directoryName);
                    }

                    if (fileName != String.Empty)
                    {
                        using (FileStream streamWriter = File.Create(outPath + "/" + theEntry.Name))
                        {

                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError("解压失敗--" + ex.Message);
        }
        
    }
}
