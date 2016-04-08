using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using ProtoCI.Core.Entities;

namespace ProtoCI.DAL
{
    public class AgentInitializer : DropCreateDatabaseAlways<AgentContext>
    {
        protected override void Seed(AgentContext context)
        {
            var agents = new List<Agent>
            {
                new Agent {Host = "127.0.0.1", Port = 5985, Username = ".\\build", Password = null}
            };
            agents.ForEach(s => context.Agents.Add(s));
            context.SaveChanges();
        }
    }
}