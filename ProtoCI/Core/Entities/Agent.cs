namespace ProtoCI.Core.Entities
{
    public class Agent
    {
        public int ID { get; set; }

        public AgentAllocator Allocator { get; set; }

        public AgentPlatformType PlatformType { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public string StoragePath { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public byte[] PrivateKeyBytes { get; set; }
    }
}