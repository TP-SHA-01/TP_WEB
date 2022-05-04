using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Edi.Topocean.EdiModels.Common
{
    public class AuthResBodyModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string user_agent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string client_ip { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Request request { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double start_time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Payload payload { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double end_time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double elapsed_time { get; set; }
    }

    public class Request
    {
        /// <summary>
        /// 
        /// </summary>
        public string usr { get; set; }
    }

    public class Preferred_date_format
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
        /// yyyy年m月d日
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string is_active { get; set; }
        /// <summary>
        /// Y年m月d日
        /// </summary>
        public string criteria1 { get; set; }
    }

    public class Preferred_language
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

    public class Preferred_number_format
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

    public class Global_options
    {
        /// <summary>
        /// 
        /// </summary>
        public Preferred_date_format preferred_date_format { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Preferred_language preferred_language { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Preferred_number_format preferred_number_format { get; set; }
    }

    public class Allowed_doc_categoryItem
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
        /// <summary>
        /// 
        /// </summary>
        public string criteria1 { get; set; }
    }

    public class Menu_page_layout
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
        /// <summary>
        /// 
        /// </summary>
        public string criteria1 { get; set; }
    }

    public class Role
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
        /// <summary>
        /// 
        /// </summary>
        public string criteria1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string criteria2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string criteria3 { get; set; }
    }

    public class Wbi_role
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
        /// <summary>
        /// 
        /// </summary>
        public string criteria1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string criteria2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string criteria3 { get; set; }
    }

    public class Wms_role
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
        /// <summary>
        /// 
        /// </summary>
        public string criteria1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string criteria2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string criteria3 { get; set; }
    }

    public class Options
    {
        /// <summary>
        /// 
        /// </summary>
        public List<Allowed_doc_categoryItem> allowed_doc_category { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Menu_page_layout menu_page_layout { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Role role { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Wbi_role wbi_role { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Wms_role wms_role { get; set; }
    }

    public class Employee
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string first_name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string last_name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string is_active { get; set; }
    }

    public class Ports
    {
    }

    public class Offices
    {
    }

    public class Global_settings
    {
        /// <summary>
        /// 
        /// </summary>
        public string preferred_timezone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string preferred_timezone_time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int preferred_timezone_offset { get; set; }
    }

    public class Settings
    {
        /// <summary>
        /// 
        /// </summary>
        public string all_visible_offices { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_ams_filing { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_ams_filing_checking { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_assist_access { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_batch_file_download { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_booking_invoice_detail { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_booking_session_update { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_chassis_no_update { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_customer_management { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_customs_session_update { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_delivery_appt_session_update { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_dev_access { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_edit_freight_invoice_status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_extended_final_ata_session_update { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_final_ata_session_update { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_freight_center_update { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_freight_cost_viewing { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_isf_filing { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_isf_monitor_access { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_login_management { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_modify_documents { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_notification_access { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_po_cost_viewing { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_po850_access { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_po850_edit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_po850_insert { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_po850_management_access { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_po850_monitor_access { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_port_session_update { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_qc_inspection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_qc_inspector_access { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_ramp_session_update { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_release_topocean_ams { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_set_flag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_systems_session_access { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_task_management { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_telex_release { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_training_access { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_usda_session_update { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_vgm_monitor_access { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_view_booking_locator { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_view_change_log { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_view_commercial_invoice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_view_customer_documents { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_view_customs_data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_view_destination_locator { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_view_documents { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_view_freight_invoice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_view_ipi { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_view_kpi { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_view_mckesson_kpi { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_view_monthly_billing { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_view_order_locator { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_view_po_locator { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_view_report_locator { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_view_shipment_locator { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_view_shipment_value_report { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allow_view_vessel_schedule { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string dashboard_display_customs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string dashboard_display_ssl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string dashboard_display_usda { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string dashboard_shipment_display_setting { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string dashboard_timeliness_setting { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> dashboard_widget_type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string default_page { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string display_dash_invoice_search_flag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string display_dash_po_section_flag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string display_dashboard_report { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string document_upload_type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string editable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string enable_dashboard_widget { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string global_view_level { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string universal_set_vessel_enable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string wbi_accept_booking { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string wbi_allow_asn_processor_access { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string wbi_allow_booking_creation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string wbi_allow_consolidation_tool_access { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string wbi_allow_titan_export_access { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string wbi_allow_topol_export_access { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string wbi_allow_view_motor_edi_activity_list { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string wbi_booking_access { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string wbi_booking_interface_format { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string wbi_data_check_enable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string wbi_default_page { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string wbi_edit_booking_confirmation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string wbi_edit_po_line_version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string wbi_universal_set_vessel_enable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string wms_default_page { get; set; }
    }

    public class Large_settings
    {
        /// <summary>
        /// 
        /// </summary>
        public string user_add_on_sql_ext { get; set; }
    }

    public class User
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string first_name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string last_name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string is_active { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string memo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int session_expire { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Global_options global_options { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Options options { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int employee_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Employee employee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Ports ports { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Offices offices { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Global_settings global_settings { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Settings settings { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Large_settings large_settings { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> entities { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> sub_entities { get; set; }
    }

    public class Payload
    {
        /// <summary>
        /// 
        /// </summary>
        public User user { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string refresh_token { get; set; }
    }


}
