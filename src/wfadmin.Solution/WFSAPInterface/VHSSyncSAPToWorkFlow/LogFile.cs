using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VHSSyncSAPToWorkflow
{
    class LogFile
    {
        //private const string LOG_FILE_PATH = @"c:\windows\system32\SyncSAPToWorkflow.log";
        private const string LOG_FILE_PATH = @"C:\Users\lzhengb\Desktop\logTest\test.log";
        public static void WriteLog(string content)
        {
            //FileStream file = null;
            StreamWriter sw = null;
            try
            {
                // file = new FileStream(LOG_FILE_PATH, FileMode.OpenOrCreate);
                sw = new StreamWriter(LOG_FILE_PATH, true);
                sw.WriteLine(string.Format("[{0}]   {1}", System.DateTime.Now, content));
            }
            catch (IOException e)
            {
                if (sw != null)
                    sw.WriteLine(string.Format("[{0}]!!!!!!Exception {1}", System.DateTime.Now, e.StackTrace));
            }
            finally
            {
                if (sw != null)
                    sw.Close();
            }
        }
    }
}
