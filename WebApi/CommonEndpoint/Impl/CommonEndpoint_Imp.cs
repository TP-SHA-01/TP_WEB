using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApi.CommonEndpoint.Interface;
using WebApi.CommonEndpoint.Model;
using WebApi.Edi.Topocean.Edi_Impl;

namespace WebApi.CommonEndpoint.Impl
{
    public class CommonEndpoint_Imp : Base_EDI_Impl, ICommonEndpoint
    {
        private string entity_filter_type { get; set; }
        private string search_term { get; set; }

        public void initSetting(ApiOptionVo optionVo)
        {
            this.entity_filter_type = optionVo.entity_filter_type;
            this.search_term = optionVo.search_term;
        }
        
    }
}