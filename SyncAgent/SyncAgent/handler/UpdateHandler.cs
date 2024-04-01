
using System;
using System.IO;
using System.Net;
using RabbitMQ.Client;
using System.Diagnostics;
using System.IO.Compression;
using System.Configuration;
using SyncAgent.common;

namespace SyncAgent {
    public class UpdateHandler : ConsumeHandler {
        
        private static string TMP_PATH  = Path.GetTempPath() + "SyncAgent\\";
        private static string server    = ConfigurationManager.AppSettings["server"];

        public void handle (IModel channel, ulong dTag, string version) {
            deleteTemp();
            download(version);
            scriptExecute();

            channel.BasicAck(dTag, false);
        }

        public void deleteTemp () {
            try {
                if (Directory.Exists(TMP_PATH)) {
                    Directory.Delete(TMP_PATH, true);
                }
            } 
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        public void scriptExecute () {
            if (!File.Exists(TMP_PATH + "script")) {
                return;
            }

            Engine.execute(TMP_PATH + "script");
        }
        
        public void download (string version) {
            if (!Directory.Exists(TMP_PATH)) {
                Directory.CreateDirectory(TMP_PATH);
            }

            using (WebClient webClient = new WebClient()) {
                try {
                    Log.info("request=" + server + version);
                    webClient.DownloadFile(server + version, TMP_PATH + version + ".zip");
                } 
                catch (Exception ex) {
                    Log.error(ex.Message);
                }
            }

            try {
                ZipFile.ExtractToDirectory(TMP_PATH + version + ".zip", TMP_PATH);
                File.Delete(TMP_PATH + version + ".zip");

            } 
            catch (Exception e) {
                Log.error(e.Message);
            }
            
            Log.info("업데이트 파일 압축 해제 완료");
        }
    }
}
