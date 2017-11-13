using Microsoft.Win32;
using RemoteConnectionManager.Core.Connections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RemoteConnectionManager.ExternalProcess
{
    public static class PuTTYImport
    {
        private const string Sessions = @"Software\SimonTatham\PuTTY\Sessions";

        public static CategoryItem[] ImportSessions()
        {
            var importedSessions = new List<CategoryItem>();

            var sessionsKey = Registry.CurrentUser.OpenSubKey(Sessions);
            if (sessionsKey == null || sessionsKey.SubKeyCount == 0)
            {
                importedSessions.ToArray();
            }

            var sessionsKeys = sessionsKey
                .GetSubKeyNames()
                .Select(x => new { Name = x, Key = sessionsKey.OpenSubKey(x) })
                .ToArray();
            foreach (var tuple in sessionsKeys)
            {
                var sk = tuple.Key;
                var hostName = sk.GetValue("HostName", null) as string;
                if (string.IsNullOrEmpty(hostName))
                {
                    continue;
                }

                var userName = sk.GetValue("UserName", null) as string;
                var publicKeyFile = sk.GetValue("PublicKeyFile", null) as string;

                importedSessions.Add(new CategoryItem
                {
                    DisplayName = Uri.UnescapeDataString(tuple.Name),
                    ConnectionSettings = new ConnectionSettings
                    {
                        Server = hostName,
                        Protocol = Protocol.Ssh,
                        Username = userName,
                        KeyFile = publicKeyFile
                    }
                });
            }

            return importedSessions.ToArray();
        }
    }
}
