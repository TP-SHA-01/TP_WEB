using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.SessionState;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApi.Edi.Topocean.EdiModels.Common;

namespace WebApi.Edi.Common
{
    public class RestApiVisitHelper : IRequiresSessionState
    {
        // Request header attribute name of authentication token
        public String TokenHeaderName { get; set; }
        // Default authentication token information
        public String DefaultToken { get; set; }
        public String ApiType { get; set; }

        public RestApiVisitHelper()
        {

        }

        // Set the request header attribute name of the authentication token during construction
        public RestApiVisitHelper(String tokenHeaderName)
        {
            TokenHeaderName = tokenHeaderName;
        }

        // Set the request header attribute name of the authentication token and the default token value during construction
        public RestApiVisitHelper(String tokenHeaderName, String defaultToken,string ApiType="JWT")
        {
            TokenHeaderName = tokenHeaderName;
            DefaultToken = defaultToken;
            this.ApiType = ApiType;
        }

        public String Get(string apiUrl, Hashtable queryParams)
        {
            return Get(apiUrl, queryParams, DefaultToken);
        }

        public String Get(string apiUrl, Hashtable queryParams, String token)
        {
            System.Net.WebClient webClientObj = CreateWebClient(token);

            apiUrl = FormatUrl(apiUrl, queryParams);
            try
            {
                String result = webClientObj.DownloadString(apiUrl);
                return result;
            }
            catch (Exception ce)
            {
                return WhenError(ce);
            }
        }

        /// <summary>
        /// Post Api Return the result text, use the default authentication token
        /// </summary>
        /// <param name="apiUrl"></param>
        /// <param name="queryParams"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public String Post(string apiUrl, Hashtable queryParams, JObject body)
        {
            return Post(apiUrl, queryParams, body, DefaultToken);
        }

        public String Post(string apiUrl, Hashtable queryParams, string body)
        {
            return Post(apiUrl, queryParams, body, DefaultToken);
        }

        /// <summary>
        /// Post Api Return result text
        /// </summary>
        /// <param name="apiUrl"></param>
        /// <param name="queryParams"></param>
        /// <param name="body"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public String Post(string apiUrl, Hashtable queryParams, JObject body, String token)
        {
            System.Net.WebClient webClientObj = CreateWebClient(token);

            apiUrl = FormatUrl(apiUrl, queryParams);
            try
            {
                String result = webClientObj.UploadString(apiUrl, "POST", body.ToString(Newtonsoft.Json.Formatting.None));
                return result;
            }
            catch (Exception ce)
            {
                return WhenError(ce);
            }
        }

        public String Post(string apiUrl, Hashtable queryParams, string body, String token)
        {
            System.Net.WebClient webClientObj = CreateWebClient(token);

            apiUrl = FormatUrl(apiUrl, queryParams);
            try
            {
                String result = webClientObj.UploadString(apiUrl, "POST", body);
                return result;
            }
            catch (Exception ce)
            {
                return WhenError(ce);
            }
        }

        public String Put(string apiUrl, Hashtable queryParams, JObject body)
        {
            return Put(apiUrl, queryParams, body, DefaultToken);
        }

        public String Put(string apiUrl, Hashtable queryParams, JObject body, String token)
        {
            System.Net.WebClient webClientObj = CreateWebClient(token);

            apiUrl = FormatUrl(apiUrl, queryParams);
            try
            {
                String result = webClientObj.UploadString(apiUrl, "PUT", body.ToString(Newtonsoft.Json.Formatting.None));
                return result;
            }
            catch (Exception ce)
            {
                return WhenError(ce);
            }
        }

        private String _Delete(string apiUrl, Hashtable queryParams, String token)
        {
            WebClient webClientObj = CreateWebClient(token);

            apiUrl = FormatUrl(apiUrl, queryParams);

            try
            {
                String result = webClientObj.UploadString(apiUrl, "DELETE", "");
                return result;
            }
            catch (Exception ce)
            {
                return WhenError(ce);
            }
        }


        public bool CheckTokenExpirse(string rep)
        {
            bool ret = false;

            try
            {
                JObject reqJObject = JObject.Parse(rep);

                int status = Int32.Parse(reqJObject["status"].ToString().Trim());

                if (status == BaseCont.HTTP_419_AUTH_TIMEOUT)
                {
                    ret = true;
                }
            }
            catch (Exception e)
            {
                ret = true;
            }

            return ret;
        }

        public AuthenticationEntity GetRefreshToken(AuthenticationEntity au)
        {
            AuthenticationEntity authentication = new AuthenticationEntity();

            try
            {
                AuthenticationModel authenticationModel = new AuthenticationModel(new AuthenticationEntity() { usr = au.usr, psd = au.psd });
                authentication = authenticationModel.Put_GetAccessToken(new AuthenticationEntity()
                {
                    access_token = au.access_token,
                    refresh_token = au.refresh_token
                });
            }
            catch (Exception e)
            {

            }

            return authentication;
        }

        #region Private Method

        // Create WebClient and set the token information
        private WebClient CreateWebClient(String token)
        {
            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            WebClient webClientObj = new WebClient();
            webClientObj.Headers.Add("Accept", "application/json");
            webClientObj.Headers.Add("X-Topocean-App", BaseCont.WBISRV_APP_CODE);
            if (!String.IsNullOrEmpty(TokenHeaderName) && !String.IsNullOrEmpty(token))
            {
                if (ApiType == "JWT")
                {
                    webClientObj.Headers.Add(TokenHeaderName, BaseCont.JWTHead + token);
                }
                else
                {
                    webClientObj.Headers.Add(TokenHeaderName, token);
                }
            }
            webClientObj.Encoding = Encoding.UTF8;
            return webClientObj;
        }

        // Format the query parameters and stitch them to url to form the final access address
        private String FormatUrl(String apiUrl, Hashtable queryParams)
        {
            if (queryParams == null) return apiUrl;
            String queryString = "";
            foreach (var k in queryParams.Keys)
            {
                if (!String.IsNullOrEmpty(queryString))
                {
                    queryString += "&";
                }
                queryString += String.Format("{0}={1}", k, queryParams[k]);
            }
            if (!String.IsNullOrEmpty(queryString))
            {
                apiUrl += "?" + queryString;
            }
            return apiUrl;
        }

        // Information returned in case of exception: should be returned according to actual needs
        private String WhenError(Exception e)
        {
            JObject result = new JObject();
            result["err_code"] = -1;
            if (e is WebException)
            {
                var we = (WebException)e;
                if (we.Response != null)  // If there is output, it still returns the actual output
                {
                    return new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                }
                else
                {
                    result["err_msg"] = we.Message;
                }
            }
            else
            {
                result["err_msg"] = e.Message;
            }
            return result.ToString(Newtonsoft.Json.Formatting.None);
        }

        #endregion

        /// <summary>
        /// Create cache
        /// </summary>
        /// <param name = "key"> Cached Key </param>
        /// <param name = "value"> cached data </param>
        /// <param name = "dateTime"> Cache expiration time </param>
        public bool CreateCache(string key, object value, DateTime expiresAt)
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

        /// <summary>
        /// Set Cookies
        /// </summary>
        /// <param name="strName">Key</param>
        /// <param name="strValue">Value</param>
        /// <param name="strDay">expires day</param>
        /// <returns></returns>
        public bool setCookie(string strName, string strValue, int strDay)
        {
            try
            {
                HttpCookie Cookie = new HttpCookie(strName);
                //Cookie.Domain = ".xxx.com";
                Cookie.Expires = DateTime.Now.AddDays(strDay);
                Cookie.Value = strValue;
                System.Web.HttpContext.Current.Response.Cookies.Add(Cookie);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get Cookies
        /// </summary>
        /// <param name="strName">Key</param>
        /// <returns></returns>
        public string getCookie(string strName)
        {
            try
            {
                HttpCookie Cookie = System.Web.HttpContext.Current.Request.Cookies[strName];
                if (Cookie != null)
                {
                    return Cookie.Value.ToString();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                return null;
            }

        }

        /// <summary>
        /// Delete Cookies
        /// </summary>
        /// <param name="strName">Key</param>
        /// <returns></returns>
        public bool delCookie(string strName)
        {
            try
            {
                HttpCookie Cookie = new HttpCookie(strName);
                //Cookie.Domain = ".xxx.com";
                Cookie.Expires = DateTime.Now.AddDays(-1);
                System.Web.HttpContext.Current.Response.Cookies.Add(Cookie);
                return true;
            }
            catch
            {
                return false;
            }
        }
        

        public void setSession(string strSessionName, string strValue, DateTime expiresAt)
        {
            try
            {
                int tiemOut = 20;
                TimeSpan duration = expiresAt - DateTime.Now;
                HttpContext.Current.Session[strSessionName] = strValue;
                if (duration.Minutes > 0)
                {
                    tiemOut = duration.Minutes;
                }
                HttpContext.Current.Session.Timeout = tiemOut;
            }
            catch (Exception)
            {
            }

        }

        public string getSession(string strSessionName)
        {
            try
            {
                if (HttpContext.Current.Session[strSessionName] == null)
                {
                    return null;
                }

                return HttpContext.Current.Session[strSessionName].ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void delSession(string strSessionName)
        {
            try
            {
                HttpContext.Current.Session[strSessionName] = null;
            }
            catch (Exception)
            {
            }
        }
    }
}
