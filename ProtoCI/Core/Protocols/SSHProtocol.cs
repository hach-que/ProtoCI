using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ProtoCI.Core.Entities;
using Renci.SshNet;

namespace ProtoCI.Core.Protocols
{
    public class SSHProtocol : IProtocol
    {
        private Dictionary<Agent, SSHConnection> _connections;

        public SSHProtocol()
        {
            _connections = new Dictionary<Agent, SSHConnection>();
        } 

        public IConnection OpenConnection(Agent agent)
        {
            if (_connections.ContainsKey(agent))
            {
                _connections[agent].ConnectionCount++;
            }
            else
            {
                var privateKeyFile = new PrivateKeyFile(new MemoryStream(agent.PrivateKeyBytes), agent.Password);
                _connections[agent] = new SSHConnection();
                _connections[agent].Client = new SshClient(
                    agent.Host,
                    agent.Port,
                    agent.Username,
                    privateKeyFile);
                _connections[agent].Client.Connect();
            }

            return _connections[agent];
        }

        public void CloseConnection(Agent agent)
        {
            _connections[agent].ConnectionCount--;
            if (_connections[agent].ConnectionCount == 0)
            {
                _connections[agent].Client.Disconnect();
                _connections.Remove(agent);
            }
        }

        public ICommand StartCommand(IConnection connection, string command, string workingDirectory)
        {
            var sshConnection = (SSHConnection) connection;
            var startCommand =
                sshConnection.Client.CreateCommand("cd '" + workingDirectory.Replace("'", "'\"'\"'") + "' && " + command);
            var commandObj = new SSHCommand(startCommand);
            startCommand.BeginExecute(command, (r) => { commandObj.IsComplete = true; }, null);
            return commandObj;
        }

        private class SSHConnection : IConnection
        {
            public SshClient Client { get; set; }

            public int ConnectionCount { get; set; }
        }

        private class SSHCommand : ICommand
        {
            private readonly SshCommand _baseCommand;

            public SSHCommand(SshCommand baseCommand)
            {
                _baseCommand = baseCommand;
            }

            public Stream StandardOutput { get { return _baseCommand.OutputStream; } }

            public Stream StandardError { get { return _baseCommand.ExtendedOutputStream; } }

            public int ExitCode { get { return _baseCommand.ExitStatus; } }

            public bool IsComplete { get; set; }
        }
    }
}