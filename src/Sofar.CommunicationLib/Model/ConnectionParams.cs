using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sofar.CommunicationLib.Model
{
    public class ConnectionParams
    {
        // G4连接多个IP
        public List<string>? IPAdressList { get; set; }

        public int Port { get; set; }
    }

}
