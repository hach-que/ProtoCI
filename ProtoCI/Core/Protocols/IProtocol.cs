using ProtoCI.Core.Entities;

namespace ProtoCI.Core.Protocols
{
    public interface IProtocol
    {
        void CloseConnection(Agent agent);
        IConnection OpenConnection(Agent agent);
        ICommand StartCommand(IConnection connection, string command, string workingDirectory);
    }
}