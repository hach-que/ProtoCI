using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Text;
using System.Web;
using ProtoCI.Core.Entities;

namespace ProtoCI.Core.Protocols
{
    public class WinRMProtocol : IProtocol
    {
        private Dictionary<Agent, WinRMConnection> _connections;

        public WinRMProtocol()
        {
            _connections = new Dictionary<Agent, WinRMConnection>();
        }

        public void CloseConnection(Agent agent)
        {
        }

        public IConnection OpenConnection(Agent agent)
        {
            var securePassword = new SecureString();
            foreach (var c in agent.Password)
            {
                securePassword.AppendChar(c);
            }
            var credential = new PSCredential(agent.Username, securePassword);
            var connectionInfo = new WSManConnectionInfo(
                false,
                agent.Host,
                agent.Port,
                "/wsman",
                "http://schemas.microsoft.com/powershell/Microsoft.PowerShell",
                credential);
            var winrmConnection = new WinRMConnection
            {
                ConnectionInfo = connectionInfo
            };
            _connections[agent] = winrmConnection;
            return winrmConnection;
        }

        public ICommand StartCommand(IConnection connection, string command, string workingDirectory)
        {
            var winrmConnection = (WinRMConnection) connection;
            // TODO Clean up the runspace
            var runspace = RunspaceFactory.CreateRunspace(winrmConnection.ConnectionInfo);
            runspace.Open();
            runspace.CreatePipeline("cd \"" + workingDirectory + "\"").Invoke();
            var pipeline = runspace.CreatePipeline(command);
            var result = pipeline.Invoke().Select(x => x.ToString()).Aggregate((a, b) => a + b);
            runspace.Dispose();
            return new WinRMCommand
            {
                StandardError = new MemoryStream(Encoding.UTF8.GetBytes(string.Empty)),
                StandardOutput = new MemoryStream(Encoding.UTF8.GetBytes(result)),
                ExitCode = 0,
                IsComplete = true
            };
        }

        private class WinRMConnection : IConnection
        {
            public WSManConnectionInfo ConnectionInfo { get; set; }
        }

        private class WinRMCommand : ICommand
        {
            public Stream StandardOutput { get; set; }
            public Stream StandardError { get; set; }
            public int ExitCode { get; set; }
            public bool IsComplete { get; set; }
        }
    }
}