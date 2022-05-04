using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebApi.Edi.Common
{
    class CallEDIServer
    {
        /// <summary>
        /// Create cache
        /// </summary>
        /// <param name = "key"> Cached Key </param>
        /// <param name = "value"> cached data </param>
        /// <param name = "dateTime"> Cache expiration time </param>
        public static bool CreateCache(string key, object value, DateTime expiresAt)
        {
            if (string.IsNullOrEmpty(key) || value == null)
            {
                return false;
            }
            HttpRuntime.Cache.Add(key, value, null, expiresAt, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            return true;
        }

        /// <summary>
        /// Get cache
        /// </summary>
        /// <param name = "key"> </param>
        /// <returns> </returns>
        public object GetCache(string key)
        {
            return string.IsNullOrEmpty(key) ? null : HttpRuntime.Cache.Get(key);
        }

        /// <summary>
        /// Remove all cache
        /// </summary>
        /// <returns></returns>
        public bool RemoveAll()
        {
            IDictionaryEnumerator iDictionaryEnumerator = HttpRuntime.Cache.GetEnumerator();
            while (iDictionaryEnumerator.MoveNext())
            {
                HttpRuntime.Cache.Remove(Convert.ToString(iDictionaryEnumerator.Key));
            }
            return true;
        }

        public void GetToken(string url)
        {
            if (GetCache("access_token") == null)
            {
                string stoken = "";
                

                RestApiClient restApiClient = new RestApiClient(url, HttpVerbNew.POST, new ContentType().JSON);

                restApiClient.MakeRequest();





                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(stoken);
                request.Method = "GET";
                request.ContentType = "text/html;charset=utf-8";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, System.Text.Encoding.UTF8);
                string retStringtoken = myStreamReader.ReadToEnd();
                myResponseStream.Close();
                myStreamReader.Close();
                JObject retStringtokenJob = JObject.Parse(retStringtoken);
                var obj = new
                {
                    access_token = retStringtokenJob["access_token"].ToString(),
                };
                //获取当前时间
                DateTime dt = DateTime.Now.AddMinutes(110);

                Formatting microsoftDataFormatSetting = default(Formatting);
                string result = JsonConvert.SerializeObject(obj, microsoftDataFormatSetting);
                bool bl = CreateCache("access_token", result, dt);
                HttpContext.Current.Response.Write(result);
            }
            else
            {
                HttpContext.Current.Response.Write(GetCache("access_token"));
            }
        }


        public enum HttpVerbNew
        {
            GET,
            POST,
            PUT,
            DELETE
        }


        public class ContentType
        {
            public string Text = "text/plain";
            public string JSON = "application/json";
            public string Javascript = "application/javascript";
            public string XML = "application/xml";
            public string TextXML = "text/xml";
            public string HTML = "text/html";
        }


        public class RestApiClient
        {
            public string EndPoint { get; set; } 
            public HttpVerbNew Method { get; set; } 
            public string ContentType { get; set; } 
            public string PostData { get; set; }


            public RestApiClient()
            {
                EndPoint = "";
                Method = HttpVerbNew.GET;
                ContentType = "text/xml";
                PostData = "";
            }

            public RestApiClient(string endpoint, string contentType)
            {
                EndPoint = endpoint;
                Method = HttpVerbNew.GET;
                ContentType = contentType;
                PostData = "";
            }

            public RestApiClient(string endpoint, HttpVerbNew method, string contentType)
            {
                EndPoint = endpoint;
                Method = method;
                ContentType = contentType;
                PostData = "";
            }


            public RestApiClient(string endpoint, HttpVerbNew method, string contentType, string postData)
            {
                EndPoint = endpoint;
                Method = method;
                ContentType = contentType;
                PostData = postData;
            }


            private static readonly string DefaultUserAgent =
                "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";


            private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain,
                SslPolicyErrors errors)
            {
                return true; 
            }
            


            public string MakeRequest()
            {
                return MakeRequest("");
            }


            public string MakeRequest(string parameters)
            {
                var request = (HttpWebRequest) WebRequest.Create(EndPoint + parameters);


                if (EndPoint.Substring(0, 8) == "https://")
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(CheckValidationResult);
                }


                request.Method = Method.ToString();
                request.ContentLength = 0;
                request.ContentType = ContentType;


                if (!string.IsNullOrEmpty(PostData) && Method == HttpVerbNew.POST)
                {
                    var encoding = new UTF8Encoding();
                    var bytes = Encoding.GetEncoding("UTF-8").GetBytes(PostData);
                    request.ContentLength = bytes.Length;


                    using (var writeStream = request.GetRequestStream())
                    {
                        writeStream.Write(bytes, 0, bytes.Length);
                    }
                }


                if (!string.IsNullOrEmpty(PostData) && Method == HttpVerbNew.PUT)
                {
                    var encoding = new UTF8Encoding();
                    var bytes = Encoding.GetEncoding("UTF-8").GetBytes(PostData);
                    request.ContentLength = bytes.Length;


                    using (var writeStream = request.GetRequestStream())
                    {
                        writeStream.Write(bytes, 0, bytes.Length);
                    }
                }

                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    var responseValue = string.Empty;


                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                        throw new ApplicationException(message);
                    }


                    // grab the response
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                            using (var reader = new StreamReader(responseStream))
                            {
                                responseValue = reader.ReadToEnd();
                            }
                    }


                    return responseValue;
                }
            }


            public bool CheckUrl(string parameters)
            {
                bool bResult = true;

                HttpWebRequest myRequest = (HttpWebRequest) WebRequest.Create(EndPoint + parameters);
                myRequest.Method = Method.ToString();
                myRequest.Timeout = 10000;
                myRequest.AllowAutoRedirect = false;
                HttpWebResponse myResponse = (HttpWebResponse) myRequest.GetResponse();
                bResult = (myResponse.StatusCode == HttpStatusCode.OK);
                return bResult;
            }
        }
    }
}
