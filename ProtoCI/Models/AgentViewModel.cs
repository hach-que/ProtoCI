using System.Web;
using ProtoCI.Core.Entities;

namespace ProtoCI.Models
{
    public class AgentViewModel
    {
        public Agent AgentData { get; set; }

        public HttpPostedFileBase PrivateKeyFile { get; set; }
    }
}