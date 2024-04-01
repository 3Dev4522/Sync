using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace SyncAgent {
    interface ConsumeHandler {
        void handle(IModel channel, ulong ea, string version);
    }
}
