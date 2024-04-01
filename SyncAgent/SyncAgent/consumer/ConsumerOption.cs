using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncAgent {
    class ConsumerOption {
        public ConsumeHandler handle { get; set; }
        public string ip { get; set; }
        public int port { get; set; }
        public string id { get; set; }
        public string pw { get; set; }
    }
}
