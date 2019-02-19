using RemoteConnectionManager.Core.Connections;
using System;
using System.Diagnostics;

namespace RemoteConnectionManager.ExternalProcess
{
    public class PuTTYConnection : HostedProcessConnection
    {
        internal PuTTYConnection(ConnectionSettings connectionSettings, IntPtr topWindowHandle)
            : base(connectionSettings, topWindowHandle)
        {
        }

        protected override void PrepareProcess(ProcessStartInfo psi)
        {
        }

        protected override string GetProcessName()
        {
            return @"dist\putty.exe";
        }

        protected override string GetProcessArguments()
        {
            var command = "-" + Enum.GetName(typeof(Protocol), ConnectionSettings.Protocol).ToLower();

            var credentials = ConnectionSettings.GetCredentials();
            if (credentials != null)
            {
                command += " -l " + credentials.Username;
                if (!string.IsNullOrEmpty(credentials.Password))
                {
                    command += " -pw " + credentials.GetPassword();
                }
                if (!string.IsNullOrEmpty(credentials.KeyFile))
                {
                    command += " -i '" + credentials.KeyFile + "'";
                }
            }
            if (!string.IsNullOrEmpty(ConnectionSettings.Port))
            {
                command += " -P " + ConnectionSettings.Port;
            }

            command += " " + ConnectionSettings.Server;

            return command;
        }
    }
}
