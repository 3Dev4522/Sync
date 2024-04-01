using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SyncAgent {

    public partial class Service1 : ServiceBase {

        public Service1() {
            InitializeComponent();
        }

        protected override void OnStart(string[] args) {
            ConsumerOption opt = new ConsumerOption() {
                id = "guest",
                pw = "guest",
                ip = "localhost",
                port = 1234,
                handle = new UpdateHandler()
            };
            ConsumerManager.getInstance(opt).consume();
        }

        protected override void OnStop() {

        }
    }
}
