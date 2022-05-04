using System.Collections.Generic;
using System.Data;
using TB_WEB.CommonLibrary.Helpers;
using WebApi.Edi.Common;
using WebApi.Edi.Topocean.Common;

namespace WebApi.Edi.Topocean.EdiModels.Common
{

    public class Shipment
    {
        /// <summary>
        /// 
        /// </summary>
        public string shipment_id_number { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string estimated_delivery_date { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string mbl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string hbl { get; set; }


        public string booking_number { get; set; }

        public int uid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Carrier carrier { get; set; }

        public Shipment() { }

        public Shipment(DataTable dt)
        {
            ModelConvertHelper modelConvert = new ModelConvertHelper(); 
            //string[] strArr = modelConvert.retDataTableCol(new Shipment());
            DataTable shipDataTable = dt.DefaultView.ToTable(false, modelConvert.retDataTableCol(new Shipment()));
            Shipment shipModel = modelConvert.SetModelValue(new Shipment(), shipDataTable);

            this.booking_number = CommonUnit.CheckEmpty(shipModel.booking_number);
            this.estimated_delivery_date = CommonUnit.CheckEmpty(shipModel.estimated_delivery_date);
            this.mbl = CommonUnit.CheckEmpty(shipModel.mbl);
            this.hbl = CommonUnit.CheckEmpty(shipModel.hbl);
            this.shipment_id_number = CommonUnit.CheckEmpty(shipModel.shipment_id_number);

            Carrier carriierModel = new Carrier();
            carriierModel.scac = CommonUnit.CheckEmpty(dt.Rows[0]["scac"]);
            carriierModel.contact = GetContactMail();
            this.carrier = carriierModel;
        }

        DBHelper dbHelper = new DBHelper();
        public List<Contact> GetContactMail()
        {
            List<Contact> listContact = new List<Contact>();

            string sql = " select top 1 emails from emaillist Where ProfileName = 'TopoceanUtil_DevErrorMsg' and IsActive = 'ACTIVE'  ";
            DataTable dt = dbHelper.ExecDataTable(sql);


            if (dt.Rows.Count > 0)
            {

                string[] arrEmails = CommonUnit.CheckEmpty(dt.Rows[0]["emails"]).Split(',');
                for (int i = 0; i < arrEmails.Length; i++)
                {
                    listContact.Add(new Contact() { email = arrEmails[i] });
                }
            }

            return listContact;
        }

        public class Carrier
        {
            /// <summary>
            /// 
            /// </summary>
            public string scac { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public List<Contact> contact { get; set; }
        }

        public class Contact
        {
            /// <summary>
            /// 
            /// </summary>
            public string email { get; set; }
        }
    }


}
