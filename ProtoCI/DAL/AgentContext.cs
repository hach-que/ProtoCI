using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using ProtoCI.Core.Entities;

namespace ProtoCI.DAL
{
    public class AgentContext : DbContext
    {
        public AgentContext() : base("DefaultConnection")
        {
        }

        public DbSet<Agent> Agents { get; set; }

        public DbSet<AgentAllocator> AgentAllocators { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}