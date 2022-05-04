using System.Configuration;
using System.Reflection;

namespace WebApi.Edi.Common
{
    /// <summary>
    /// Object Factory
    /// </summary>
    public class ReflectionEDIFactory
    {
        // Configuration information
        private static string bizName = ConfigurationManager.AppSettings["bizName"].ToString();
        //private static string bizName = "TBS_Library";

        public static T CreateObject<T>(string className)
        {
            // TODO
            return (T)Assembly.Load(bizName).CreateInstance(bizName + "." + className);
        }
    }
}
