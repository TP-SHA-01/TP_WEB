using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Edi.Topocean.EdiModels.Common;


namespace WebApi.Edi.Topocean.EdiModels.Office_API
{
    public class OfficeModel : BaseReponseModel<OfficeModel>
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string is_active { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string origin_lot_no { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string destination_lot_no { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string titan_edi_reference_code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int type_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Type type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int parent_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Parent parent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int origin_handling_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Origin_handling origin_handling { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int destination_handling_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Destination_handling destination_handling { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string mcs_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string inttra_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string address1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string address2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string address3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string city { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string city_code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string state { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string sub_division_code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<int> locations { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int region_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Region region { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int country_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Country country { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string is_manual_hbl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Settings settings { get; set; }



        public class Request
        {
        }

        public class Type
        {
            /// <summary>
            /// 
            /// </summary>
            public int id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string type { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string value { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string is_active { get; set; }
        }

        public class Parent
        {
        }

        public class Origin_handling
        {
        }

        public class Destination_handling
        {
        }

        public class Region
        {
        }

        public class Country
        {
        }

        public class Settings
        {
        }
    }
}
