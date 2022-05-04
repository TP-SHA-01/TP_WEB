using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApi.Edi.Topocean.Edi_Interface;
using WebApi.Edi.Topocean.EdiModels.Common;
using System.Reflection;
using System.Data;
using System.Data.OleDb;
using WebApi.Edi.Topocean.EdiModels.OpenTrack_API;
using TB_WEB.CommonLibrary.Helpers;
using WebApi.Edi.Topocean.Common;
using WebApi.Edi.Common;
using TB_WEB.CommonLibrary.Log;

namespace WebApi.Edi.Topocean.Edi_Impl
{
    public class Base_EDI_Impl : IBase_EDI_Response
    {
        private string AuthenUser { get; set; }
        private string access_token { get; set; }
        private string refresh_token { get; set; }
        private string apiUrl { get; set; }
        private string tokenHeadName { get; set; }
        public string repJson { get; set; }

        protected DataTable BaseDataTable { get; set; }
        public Hashtable queryParams { get; set; }
        public string body { get; set; }
        public ParameterEntity entity { get; set; }

        DBHelper dbHelper = new DBHelper();
        AuthenticationModel auth = new AuthenticationModel();
        private RestApiVisitHelper apiVisitHelper = new RestApiVisitHelper();
        ModelConvertHelper modelConvert = new ModelConvertHelper();

        public void init(EDIPostEntity baseEntity, AuthenticationModel auEntity)
        {
            AuthenticationEntity auth = auEntity.Post_GetAccessToken();

            this.access_token = auth.access_token;
            this.refresh_token = auth.refresh_token;
            this.tokenHeadName = auEntity.tokenHeadName;
            this.auth = auEntity;
            this.apiUrl = baseEntity.apiUrl;
            this.body = baseEntity.body;
            this.queryParams = baseEntity.queryParams;
            this.AuthenUser = auEntity.AuthenUser;
        }

        public string repPOSTJson() {
            return this.repJson;
        }

        public dynamic hydrate()
        {
            var jsonModel = JsonToObject(this.repJson);
            return jsonModel;
        }

        public string serialize()
        {
            return "";
        }

        public void filter(Dictionary<string, string> dicValue)
        {
            string urlPare = String.Empty;
            string url = String.Empty;

            foreach (KeyValuePair<string, string> kvp in dicValue)
            {
                urlPare = kvp.Key + "=" + kvp.Value + "&";
            }

            url = "?" + urlPare.TrimEnd('&');

            if (!String.IsNullOrEmpty(this.apiUrl))
            {
                this.apiUrl = this.apiUrl + url;
            }
            else
            {

            }

            Get();

        }

        public bool CheckAccess(AuthenticationModel auEntity)
        {
            bool ret = false;

            this.AuthenUser = auEntity.AuthenUser;

            if (AuthenUser == "true" && CheckIPAccess())
            {
                ret = true;
                AuthenticationEntity auth = auEntity.Post_GetAccessToken();

                this.access_token = auth.access_token;
                this.refresh_token = auth.refresh_token;

            }

            return ret;
        }

        private bool CheckIPAccess()
        {
            bool ret = false;

            try
            {
                string remoteAddress = CommonUnit.GetIP();

                //ArrayList allowedIPDict = new ArrayList();
                List<string> allowedIPDict = new List<string>();
                allowedIPDict.Add("127.0.0.1");             // Self
                allowedIPDict.Add("10.5.27.48");            // Internal US Office
                allowedIPDict.Add("10.6.5.191");            // Internal US Office
                allowedIPDict.Add("10.5.27.1");             // Internal US Office
                allowedIPDict.Add("10.6.5.192");            // Internal US Office
                allowedIPDict.Add("12.110.188.200");        // US Office
                allowedIPDict.Add("12.110.188.210");        // US Office App server
                allowedIPDict.Add("12.110.188.84");         // US Office
                allowedIPDict.Add("54.70.20.224");          // US Office VPN
                allowedIPDict.Add("35.164.86.237");         // US Titan AWS Seattle
                allowedIPDict.Add("54.169.54.137");         // App server SG
                allowedIPDict.Add("106.15.181.150");        // App server SHA
                allowedIPDict.Add("34.217.233.104");        // Dev server USA
                allowedIPDict.Add("45.51.66.15");           // Jon C home
                allowedIPDict.Add("116.228.245.134");       // SHA
                allowedIPDict.Add("218.17.57.217");         // SZN
                allowedIPDict.Add("45.116.9.229");          // SZN VPN
                allowedIPDict.Add("61.220.204.223");        // TWN OFFICE VPN
                allowedIPDict.Add("61.222.206.197");        // TWN OFFICE VPN
                allowedIPDict.Add("210.176.173.117");       // HKG
                allowedIPDict.Add("223.197.202.144");       // HKG 2
                allowedIPDict.Add("103.77.192.227");        // SHA
                allowedIPDict.Add("45.116.9.243");          // SHA
                allowedIPDict.Add("114.86.219.19");         // SHA - Ethan Shen
                allowedIPDict.Add("101.231.115.110");       // SHA - Linda Ma 1
                allowedIPDict.Add("101.86.16.20");          // SHA - Linda Ma 2
                allowedIPDict.Add("101.86.23.100");         // SHA - Linda Ma 3

                if (allowedIPDict.Contains(remoteAddress))
                {
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("Exception Message -- > " + ex.Message + " ; StackTrace --> :" + ex.StackTrace);
            }

            return ret;
        }

        private void getToken()
        {
            apiVisitHelper = new RestApiVisitHelper(tokenHeadName, access_token);
            string rep = apiVisitHelper.Post(apiUrl, null, body);

            JObject reqJObject = JObject.Parse(rep);

            int status = Int32.Parse(reqJObject["status"].ToString().Trim());

            if (status == BaseCont.HTTP_419_AUTH_TIMEOUT)
            {
                AuthenticationEntity authentication = auth.Put_GetAccessToken(new AuthenticationEntity() { access_token = this.access_token, refresh_token = this.refresh_token });

                this.access_token = authentication.access_token;
                this.refresh_token = authentication.refresh_token;
            }
        }

        public void Post()
        {
            string rep = String.Empty;

            try
            {
                apiVisitHelper = new RestApiVisitHelper(tokenHeadName, access_token);
                rep = apiVisitHelper.Post(apiUrl, null, body);

                if (apiVisitHelper.CheckTokenExpirse(rep))
                {
                    AuthenticationEntity auValue = apiVisitHelper.GetRefreshToken(new AuthenticationEntity()
                    {
                        access_token = auth.access_token
                        ,
                        psd = auth.psd
                        ,
                        refresh_token = auth.refresh_token
                        ,
                        usr = auth.usr
                    });

                    apiVisitHelper = new RestApiVisitHelper(tokenHeadName, auValue.access_token);
                    rep = apiVisitHelper.Post(apiUrl, null, body);
                }

            }
            catch (Exception e)
            {
                rep = e.Message;
            }

            this.repJson = rep;
        }

        public void Get()
        {
            string rep = String.Empty;

            try
            {
                apiVisitHelper = new RestApiVisitHelper(tokenHeadName, access_token);
                rep = apiVisitHelper.Get(apiUrl, queryParams);

                if (apiVisitHelper.CheckTokenExpirse(rep))
                {
                    AuthenticationEntity auValue = apiVisitHelper.GetRefreshToken(new AuthenticationEntity()
                    {
                        access_token = auth.access_token
                        ,
                        psd = auth.psd
                        ,
                        refresh_token = auth.refresh_token
                        ,
                        usr = auth.usr
                    });

                    apiVisitHelper = new RestApiVisitHelper(tokenHeadName, auValue.access_token);
                    rep = apiVisitHelper.Get(apiUrl, queryParams);
                }

            }
            catch (Exception e)
            {
                rep = e.Message;
            }

            this.repJson = rep;
        }

        public void Put()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// JSON Convent
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public dynamic JsonToObject(string jsonString)
        {
            return JsonConvert.DeserializeObject<dynamic>(jsonString);
        }

        #region function: LoadBaseResponse ; load common field of API response into T class
        public T LoadBaseResponse<T>()
        {
            return (this.LoadBaseResponse<T>(this.repJson));
        }

        public T LoadBaseResponse<T>(string jsonString)
        {
            try
            {
                if (string.IsNullOrEmpty(jsonString))
                {
                    throw (new Exception("JSON is empty"));
                }
                
                return JsonConvert.DeserializeObject<T>(jsonString);
                
            }
            catch (Exception ex)
            {
                LogHelper.Error("Exception Message -- > " + ex.Message + " ; StackTrace --> :" + ex.StackTrace);
                return (T)Convert.ChangeType(null, typeof(T));
            }
        }
        #endregion

        #region Base API Action
        public void init(ParameterEntity parameterEntity)
        {
            this.entity = parameterEntity;
        }

        public string GetICN()
        {
            string strICN = String.Empty;
            try
            {
                string sql = String.Format(" SELECT {0} FROM EDISysInfo ", entity.icn_type);
                DataTable dt = dbHelper.ExecDataTable(sql);

                if (dt.Rows.Count > 0)
                {
                    if (String.IsNullOrEmpty(CommonUnit.CheckEmpty(dt.Rows[0][entity.icn_type])))
                    {
                        strICN = "1";
                    }
                    else
                    {
                        strICN = CommonUnit.CheckEmpty(dt.Rows[0][entity.icn_type]);
                    }

                    string udpSql = String.Format(" UPDATE EDISysInfo SET {0} = ISNULL({0},'1') + 1 ", entity.icn_type);
                    bool updQuery = dbHelper.ExecuteQuery(CommandType.Text, udpSql);
                    if (updQuery)
                    {
                        return strICN;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("Exception Message -- > " + ex.Message + " ; StackTrace --> :" + ex.StackTrace);
            }

            return strICN;
        }

        public void insertEDIMap(List<OpenTrack_POST_VO> listTerminalVO)
        {
            string edi_type = entity.edi_type;
            string insSqlMap = String.Empty;
            string insEDIArchiveSql = String.Empty;

            try
            {
                Hashtable sqlParameters = new Hashtable();
                List<OleDbParameter> ps;
                for (int i = 0; i < listTerminalVO.Count; i++)
                {
                    string strBatchDate = String.Empty;
                    string identifier3 = String.Empty;
                    string source = CommonUnit.CheckEmpty(listTerminalVO[i].source);
                    string ediContent = String.Empty;
                    string icn = String.Empty;

                    strBatchDate = CommonUnit.DateTimeFormat(listTerminalVO[i].batch_date);
                    ediContent = CommonUnit.CheckEmpty(listTerminalVO[i].confirmation_no) + "-" + CommonUnit.CheckEmpty(listTerminalVO[i].container_no); // MBL-ContainerNo
                    icn = GetICN();

                    if (source == BaseCont.OpenTrack_Source)
                    {
                        identifier3 = CommonUnit.CheckEmpty(listTerminalVO[i].container_no);
                        EDIArchiveModel model = new EDIArchiveModel();
                        model.EDIContent = ediContent;
                        model.ICN = icn;
                        model.EDIType = "OPEN_TRACK";
                        CommonUnit.InsertEDIArchive(model);                        
                    }
                    else
                    {
                        identifier3 = DBHelperBase.ToDBStr_ReplaceVbcrlf2BR(CommonUnit.CheckEmpty(listTerminalVO[i].hbl));
                    }

                    insSqlMap = String.Format(" DECLARE @strBookingConfirmation nvarchar(200) " +
                                               " SELECT  @strBookingConfirmation = BookingConfirmation FROM POTracing WITH(NOLOCK) WHERE BookingReqID = '{0}'; " +
                                               " if not exists (SELECT ICN FROM  edimap WHERE EDIType = '{4}' AND  Identifier = '{0}' AND Identifier2 = @strBookingConfirmation AND Identifier3 = '{2}' AND Active = 'Y') " +
                                               "   insert into edimap (EDIType,Identifier,Identifier2,Identifier3,ICN,GeneratedDate,AlertSentDate,Active)values('{4}','{0}',@strBookingConfirmation,'{2}',{1},GETDATE(),GETDATE(),'Y');" +
                                               " else " +
                                               "   update edimap set AlertSentDate = GETDATE() WHERE EDIType = '{4}' AND  Identifier = '{0}' AND Identifier2 = @strBookingConfirmation AND Identifier3 = '{2}' AND Active = 'Y' "
                        , DBHelperBase.ToDBStr_ReplaceVbcrlf2BR(CommonUnit.CheckEmpty(listTerminalVO[i].booking_id))
                        , icn
                        , identifier3
                        , DBHelperBase.ToDBStr_ReplaceVbcrlf2BR(strBatchDate)
                        , edi_type
                    );

                    dbHelper.ExecuteQuery(CommandType.Text, insSqlMap);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("Exception Message -- > " + ex.Message + " ; StackTrace --> :" + ex.StackTrace);
            }
        }
        #endregion

    }
}
