using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ProyectoSimon.Utils
{

    /// <summary>
    /// This class is intended to be used with the default logging clase of C#. It 
    /// save the log in a log file and display a simple message to the user.
    /// </summary>
    public class CustomLog
    {
        private static TextWriter file;

        public enum LoggerLevel
        {
            DEGUB,
            ERROR,
            INFO,
            SEVERE,
            WARNING,
            CONFIG
        }

        /// <summary>
        /// Enables to log all exceptions to a file and display simple message on demand.
        /// </summary>
        /// <param name="Exception">Specific exception fired.</param>
        /// <param name="level">Logger level</param>
        /// <param name="logMessage">Message to be displayed</param>        
        public static void Log(Exception exeption, LoggerLevel level, string logMessage)
        {
            try
            {
                file = File.AppendText(String.Format(CommonConstants.LOG_FILE_PATH, DateTime.Now.ToLongDateString()));
                file.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());

                if (exeption != null)
                {
                    file.WriteLine("{0}: [{1}] {2}", exeption.Message, level.ToString().ToUpper(), logMessage);
                }
                else
                {
                    file.WriteLine("{0}: {1}", level.ToString().ToUpper(), logMessage);
                }

                file.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading from {0}. Message = {1}", CommonConstants.LOG_FILE_PATH, e.Message);
            }
        }

    }

}
