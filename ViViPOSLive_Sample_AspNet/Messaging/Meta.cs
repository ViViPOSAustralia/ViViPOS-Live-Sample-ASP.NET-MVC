using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ViViPOSLive_Sample_AspNet.Messaging
{
    public class Meta
    {
        public string type { get; set; }

        public string target { get; set; }

        public string connectionId { get; set; }

        public string verifyToken { get; set; }
    }
}