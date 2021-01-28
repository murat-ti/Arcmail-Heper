using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Arcmail.Helpers
{
    class AppConfigHelper
    {
        private Configuration configManager;
        private KeyValueConfigurationCollection confCollection;

        public AppConfigHelper()
        {
            configManager = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            confCollection = configManager.AppSettings.Settings;
        }

        public string readConfigKey(String key)
        {
            string data = "";

            try
            {
                data = confCollection[key].Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show(key + " is not found in config file\r\n"+ex.Message);
            }

            return data;
        }
    }
}
