using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SyncAgent {
    class Log {
        public static void error(string message) {
            entry(message, EventLogEntryType.Error);
        }

        public static void info(string message) {
            entry(message, EventLogEntryType.Information);
        }

        private static void entry(string message, EventLogEntryType type) {
            System.Diagnostics.EventLog eventLog = new System.Diagnostics.EventLog();

            if (!System.Diagnostics.EventLog.SourceExists("SyncAgent")) {
                System.Diagnostics.EventLog.CreateEventSource("SyncAgent", "Application");
            }

            eventLog.Source = "SyncAgent";

            int eventID = 8;

            eventLog.WriteEntry(message,
                                type,
                                eventID);

            eventLog.Close();
        }
    }
}
