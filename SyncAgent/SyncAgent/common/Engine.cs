using System;
using System.IO;
using System.Diagnostics;

namespace SyncAgent.common {
    class Engine {

        private static string TMP_PATH = Path.GetTempPath() + "SyncAgent\\";

        public static void execute (string scriptPath) {
            string[] lines = File.ReadAllLines(scriptPath);

            foreach (string line in lines) {
                string[] parts = line.Split(' ');
                
                if (parts.Length < 2) {
                    continue;
                }

                string command = parts[0].ToUpper();
                string argument = string.Join(" ", parts, 1, parts.Length - 1);
                
                switch (command) {
                    case "KILL":
                        kill(argument);
                        break;
                    case "RUN":
                        exec(argument);
                        break;
                    case "CLEAR":
                        exec("rd /S /Q " + argument + "& mkdir " + argument);
                        break;
                    case "MOV":
                        exec("robocopy " + TMP_PATH + " " + argument + " /E");
                        break;
                    case "MAIN":
                        Loader.PROCESS_INFORMATION procInfo;
                        Loader.StartProcessAndBypassUAC(argument, out procInfo);
                        break;
                }
            }
        }

        private static void kill (string name) {
            try {
                Process[] findProcess = Process.GetProcessesByName(name);
                if (findProcess.Length > 0) {
                    foreach (Process process in findProcess) {
                        process.Kill();
                    }
                }
            }
            catch (Exception e) {
                Log.error("[Engine] " + e.Message);
            }
        }

        private static void exec (string command) {
            try {
                Process runner = Process.Start("cmd.exe", $"/c {command}");
                runner.WaitForExit();
            }
            catch (Exception e) {
                Log.error("[Engine] " + e.Message);
            }
        }
    }
}
