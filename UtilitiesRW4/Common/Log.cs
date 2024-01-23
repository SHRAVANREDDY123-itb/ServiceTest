using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWUtilities.Common
{
    public static class Log
    {
        private static object mlock = new object();

        static Log()
        {
            if (!System.Diagnostics.EventLog.Exists("RW3"))
            {
                System.Diagnostics.EventLog.CreateEventSource("RW", "RW3");
            }
        }

        /// <summary>
        /// Write function
        /// </summary>
        /// <param name="ex">This is Exception object.(Default SourceName is RW3)</param>
        public static void write(Exception ex)
        {
            try
            {
                write("RW3", ex);
            }
            catch
            {
                ////ignore
            }
        }

        /// <summary>
        /// Write function
        /// </summary>
        /// <param name="Message">This is Any String Message.(Default SourceName is RW3)</param>
        public static void write(string Message)
        {
            try
            {
                write("RW3", Message);
            }
            catch
            {
                ////ignore
            }
        }

        /// <summary>
        /// Write function
        /// </summary>
        /// <param name="Source">This is Event Source Name.(Must exist)</param>
        /// <param name="ex">This is Exception object.</param>
        public static void write(string Source, Exception ex)
        {
            try
            {
                lock (mlock)
                {
                    EventLog.WriteEntry(Source, ex.Message + "\\n" + ex.StackTrace + "\\n" + ex.InnerException, EventLogEntryType.Error);
                }
            }
            catch
            {
                ////ignore
            }
        }

        /// <summary>
        /// Write function
        /// </summary>
        /// <param name="Source">This is Event Source Name.(Must exist)</param>
        /// <param name="Message">This is Any String Message.</param>
        public static void write(string Source, string Message)
        {
            try
            {
                lock (mlock)
                {
                    EventLog.WriteEntry(Source, Message);
                }
            }
            catch
            {
                ////ignore
            }
        }
    }
}
