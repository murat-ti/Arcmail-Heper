using System;
using System.IO;
using System.Windows.Forms;

namespace Arcmail.Helpers
{
    class LogHelper
    {
        private AppConfigHelper appConfigHelper;
        private string logPath;
        public LogHelper()
        {
            appConfigHelper = new AppConfigHelper();
            logPath = appConfigHelper.readConfigKey("logPath");
        }
        public void showLog(String log, Exception ex = null, bool showMessageBox = false)
        {
            string result = DateTime.Now.ToString("hh:mm:ss") + "\t" + log;
            if (ex != null)
                result = "\r\n" + ex.ToString();

            //Console.Write(result);

            if (showMessageBox)
                MessageBox.Show(result);

            WriteToFile(result);
        }

        //log all in and out file names
        public void fileLog(string filename, string subdir)
        {
            if(filename != "")
                WriteToFile(filename, subdir);
        }

        public void WriteToFile(string message, string subdir = "")
        {
            if (logPath != "")
            {
                string today = DateTime.Now.ToString("yyyy-MM-dd");
                string filename = today + ".txt";
                string path;

                if (!Directory.Exists(logPath))
                    Directory.CreateDirectory(logPath);

                if (subdir != "")
                {
                    path = String.Format("{0}/{1}/{2}", logPath, subdir, filename);
                    if (!Directory.Exists(logPath+"\\"+subdir))
                        Directory.CreateDirectory(logPath + "\\" + subdir);
                }
                else
                    path = logPath + filename;
                

                if (!File.Exists(path))
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(message);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine(message);
                    }
                    
                }
            }
        }
    }
}
