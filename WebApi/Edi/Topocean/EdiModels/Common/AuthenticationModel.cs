using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TB_WEB.CommonLibrary.CommonFun;
using TB_WEB.CommonLibrary.Log;
using WebApi.Edi.Common;

namespace WebApi.Edi.Topocean.EdiModels.Common
{
    public class AuthenticationModel
    {
        protected string tokenApiUrl = BaseCont.ApiUrl + "/authentication";
        public string tokenHeadName = BaseCont.tokenHeadName;

        RestApiVisitHelper apiVisitHelper = new RestApiVisitHelper();

        public string usr { get; set; }
        public string psd { get; set; }
        public string AuthenUser { get; set; }

        public string access_token { get; set; }
        public string refresh_token { get; set; }
        private int session_expire { get; set; }

        public AuthenticationModel() { }

        public AuthenticationModel(AuthenticationEntity authenticationEntity)
        {
            this.usr = authenticationEntity.usr;
            this.psd = authenticationEntity.psd;
        }

        public AuthenticationEntity Post_GetAccessToken()
        {
            try
            {
                JObject authObject = new JObject();
                authObject.Add("usr", usr);
                authObject.Add("psd", psd);

                GetAccessToken("POST", authObject);
            }
            catch (Exception e)
            {
                LogHelper.Error("Post_GetAccessToken : " + e.Message);
                Console.WriteLine(e);
                throw;
            }

            return new AuthenticationEntity(){access_token = this.access_token,refresh_token = this.refresh_token};
        }

        public AuthenticationEntity Put_GetAccessToken(AuthenticationEntity au)
        {
            try
            {
                JObject authObject = new JObject();
                authObject.Add("access_token", au.access_token);
                GetAccessToken("PUT", authObject, au.refresh_token);
            }
            catch (Exception e)
            {
                LogHelper.Error("Put_GetAccessToken : " + e.Message);
                Console.WriteLine(e);
                throw;
            }

            return new AuthenticationEntity() { access_token = this.access_token, refresh_token = this.refresh_token };
        }

        private void GetAccessToken(string type, JObject authObject,string _refresh_token = "")
        {
            string retStringtoken = String.Empty;

            try
            {
                if (MemoryCacheUtil.GetValue("access_token") == null)
                {
                    if (type == "POST")
                    {
                        retStringtoken = apiVisitHelper.Post(tokenApiUrl, null, authObject, null);
                    }
                    else if (type == "PUT")
                    {
                        apiVisitHelper = new RestApiVisitHelper(tokenHeadName, _refresh_token);
                        retStringtoken = apiVisitHelper.Put(tokenApiUrl, null, authObject, null);
                    }
                    LogHelper.Debug("GetAccessToken => retStringtoken: " + retStringtoken);
                    JObject retStringtokenJob = JObject.Parse(retStringtoken);

                    JObject useJObject = JObject.Parse(retStringtokenJob["payload"]["user"].ToString());

                    var obj = new
                    {
                        access_token = retStringtokenJob["payload"]["access_token"].ToString(),
                        refresh_token = retStringtokenJob["payload"]["refresh_token"].ToString(),
                    };
                     
                    var session_expire_obj = new { session_expire = (int)useJObject["session_expire"] };

                    this.session_expire = session_expire_obj.session_expire ;

                    int tiemOut = 0;
                    DateTime dt = DateTime.Now.AddSeconds(session_expire_obj.session_expire);
                    TimeSpan duration = dt - DateTime.Now;
                    if (duration.Minutes > 0)
                    {
                        tiemOut = duration.Minutes;
                    }
                    Formatting microsoftDataFormatSetting = default(Formatting);
                    this.access_token = obj.access_token;
                    this.refresh_token = obj.refresh_token;

                    MemoryCacheUtil.SetValue("access_token", obj.access_token, tiemOut);
                    MemoryCacheUtil.SetValue("refresh_token", obj.refresh_token, tiemOut);
                }
                else
                {
                    this.access_token = CommonUnit.CheckEmpty(MemoryCacheUtil.GetValue("access_token"));
                    this.refresh_token = CommonUnit.CheckEmpty(MemoryCacheUtil.GetValue("refresh_token"));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                LogHelper.Error("GetAccessToken : " + e.Message + ", retStringtoken" + retStringtoken);
                throw;
            }
        }

    }
}
