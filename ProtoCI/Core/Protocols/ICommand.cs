using System.IO;

namespace ProtoCI.Core.Protocols
{
    public interface ICommand
    {
        Stream StandardOutput { get; }

        Stream StandardError { get; }

        int ExitCode { get; }

        bool IsComplete { get; }
    }
}