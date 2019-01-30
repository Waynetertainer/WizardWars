using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NET_System
{
    public static class NET_Debug
    {
        static int errorsCount = 0;
        static bool log = true;
        static Queue<string> debugMessages = new Queue<string>();

        public static string ReturnMessages()
        {
            if (debugMessages.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                lock (debugMessages)
                {
                    while (debugMessages.Count > 0)
                    {
                        sb.AppendLine(debugMessages.Dequeue());
                    }
                    return sb.ToString();
                }
            }
            else
            {
                return "";
            }
        }
        public static void DeactivateLogging()
        {
            log = false;
        }
        public static void InjectMessage(string system, string message)
        {
            lock (debugMessages)
            {
                debugMessages.Enqueue(system + " " + message);
                WriteLog(system, message);
            }
        }
        private static void WriteLog(string system, string note)
        {
            if (log == true)
            {
                //Create logs folder if not exists
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "//" + @"AQNET_Logs//";
                DirectoryInfo folder = new DirectoryInfo(path);
                if (!folder.Exists)
                {
                    folder.Create();
                }
                //Write log
                string time = System.DateTime.Now.TimeOfDay.ToString();
                string text = "\n" + "[" + time + "]" + " " + "[" + system + "]" + "\t\t" + "{" + note + "}";
                System.IO.File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "//" + @"AQNET_Logs//Log_" + system + ".txt", text);
            }
        }
    }
}
