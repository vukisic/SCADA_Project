using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Common
{
    public class ConfigurationReader
    {
        public static string ReadValue(ServiceContext context, string sectionName, string paramName, string package = "Config")
        {
            try
            {
                return context.CodePackageActivationContext.GetConfigurationPackageObject(package).Settings.Sections[sectionName].Parameters[paramName].Value;
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
