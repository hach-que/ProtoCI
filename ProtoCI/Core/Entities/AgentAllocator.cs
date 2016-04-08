using System.Collections.Generic;

namespace ProtoCI.Core.Entities
{
    public class AgentAllocator
    {
        public int ID { get; set; }

        public string AgentAllocatorImplementationType { get; set; }

        public virtual ICollection<Agent> Agents { get; set; } 
    }
}