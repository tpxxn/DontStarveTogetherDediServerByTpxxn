using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

// 经作者同意，代码改自
// https://github.com/vvwall/DSTseverTools

namespace 饥荒开服工具ByTpxxn.Class.Tools
{
    public static class HttpHelper
    {
        public static string HttpPost(string Url, string postDataStr)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Proxy = null;
                var cookieContainer = new CookieContainer();
                request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
                request.CookieContainer = cookieContainer;
                var requestStream = request.GetRequestStream();
                var streamWriter = new StreamWriter(requestStream, Encoding.GetEncoding("gb2312"));
                streamWriter.Write(postDataStr);
                streamWriter.Close();
                var httpWebResponse = (HttpWebResponse)request.GetResponse();
                httpWebResponse.Cookies = cookieContainer.GetCookies(httpWebResponse.ResponseUri);
                var responseStream = httpWebResponse.GetResponseStream();
                var streamReader = new StreamReader(responseStream ?? throw new InvalidOperationException(), Encoding.GetEncoding("utf-8"));
                var resultString = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                return resultString;
            }
            catch (Exception)
            {
                MessageBox.Show("当前网络不可用");
                return "{\"code\":\"404\"}";
            }

        }

        public static string HttpGet(string Url, string postDataStr)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                request.Timeout = 8000;
                request.Proxy = null;
                var response = (HttpWebResponse)request.GetResponse();
                var myResponseStream = response.GetResponseStream();
                var myStreamReader = new StreamReader(myResponseStream ?? throw new InvalidOperationException(), Encoding.GetEncoding("utf-8"));
                var retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();

                return retString;
            }
            catch (Exception)
            {
                return "{\"version\":1}";
            }
        }

        public static void DownloadFile(string URL, string filename, System.Windows.Controls.TextBlock textBlock)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                var totalBytes = httpWebResponse.ContentLength;
                var responseStream = httpWebResponse.GetResponseStream();
                Stream fileStream = new FileStream(filename, FileMode.Create);
                long totalDownloadedByte = 0;
                var buffer = new byte[10240];
                if (responseStream != null)
                {
                    var osize = responseStream.Read(buffer, 0, buffer.Length);
                    while (osize > 0)
                    {
                        totalDownloadedByte = osize + totalDownloadedByte;
                        System.Windows.Forms.Application.DoEvents();
                        fileStream.Write(buffer, 0, osize);
                        osize = responseStream.Read(buffer, 0, buffer.Length);
                        var percent = (float)totalDownloadedByte / (float)totalBytes * 100;
                        percent = (float)Math.Round(percent, 3);
                        textBlock.Text = "当前下载进度：" + percent.ToString(CultureInfo.InvariantCulture) + "%";
                        System.Windows.Forms.Application.DoEvents(); //必须加注这句代码，否则textBlock将因为循环执行太快而来不及显示信息
                    }
                }
                fileStream.Close();
                responseStream?.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("下载失败，可能是网络问题");
            }
        }

    }
}
