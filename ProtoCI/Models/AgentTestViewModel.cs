using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProtoCI.Models
{
    public class AgentTestViewModel
    {
        public string SystemIdentifier { get; set; }
        public Exception Exception { get; set; }
    }
}