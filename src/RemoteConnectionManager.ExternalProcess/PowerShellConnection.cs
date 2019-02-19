using RemoteConnectionManager.Core.Connections;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace RemoteConnectionManager.ExternalProcess
{
    public class PowerShellConnection : HostedProcessConnection
    {
        public PowerShellConnection(ConnectionSettings connectionSettings, IntPtr topWindowHandle)
             : base(connectionSettings, topWindowHandle)
        {
        }

        protected override void PrepareProcess(ProcessStartInfo psi)
        {
            psi.WorkingDirectory = Environment.GetEnvironmentVariable("USERPROFILE");
        }

        protected override string GetProcessName()
        {
            return @"powershell.exe";
        }

        protected override string GetProcessArguments()
        {
            var command = $"Enter-PSSession -ComputerName {ConnectionSettings.Server}";

            var credentials = ConnectionSettings.GetCredentials();
            if (credentials != null)
            {
                var username = string.IsNullOrEmpty(credentials.Domain)
                    ? $"{credentials.Username}"
                    : $"{credentials.Domain}\\{credentials.Username}";
                var credential = string.IsNullOrEmpty(credentials.Password)
                    ? $"{username}"
                    : $"(New-Object System.Management.Automation.PSCredential('{username}', '{credentials.Password}' | ConvertTo-SecureString -asPlainText -Force))";

                command += $" -Credential {credential}";

                if (!string.IsNullOrEmpty(credentials.KeyFile))
                {
                    command += $" -KeyFilePath '{credentials.KeyFile}'";
                }
            }
            if (!string.IsNullOrEmpty(ConnectionSettings.Port))
            {
                command += $" -Port {ConnectionSettings.Port}";
            }

            var commandBytes = Encoding.Unicode.GetBytes(command);
            var encodedCommand = Convert.ToBase64String(commandBytes);

            return $"-NoLogo -NoExit -EncodedCommand {encodedCommand}";
        }

        protected override void WaitForProcess(Process process)
        {
            // Note that using process.WaitForInputIdle() will
            // throw an exception because PowerShell does not have
            // a GUI.
            while (process.MainWindowHandle == IntPtr.Zero)
            {
                Thread.Sleep(10);
            }
        }
    }
}
