using Microsoft.Win32;
using RemoteConnectionManager.Core.Connections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RemoteConnectionManager.Rdp
{
    public static class RdpImport
    {
        private const string Server = "full address:s:";
        private const string Port = "server port:i:";
        private const string Domain = "domain:s:";
        private const string Username = "username:s:";

        public static CategoryItem[] ImportFiles()
        {
            var importedSessions = new List<CategoryItem>();

            var dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Filter = "*.rdp|*.rdp";

            if (dialog.ShowDialog() == true)
            {
                foreach (var fileName in dialog.FileNames)
                {
                    try
                    {
                        var lines = File.ReadAllLines(fileName);
                        var fullAddress = lines.FirstOrDefault(x => x.StartsWith(Server))?.Substring(Server.Length);
                        if (string.IsNullOrEmpty(fullAddress))
                        {
                            continue;
                        }

                        var serverParts = fullAddress.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        var server = serverParts[0];
                        var port = serverParts.Length > 1
                            ? serverParts[1]
                            : lines.FirstOrDefault(x => x.StartsWith(Port))?.Substring(Port.Length);
                        var domain = lines.FirstOrDefault(x => x.StartsWith(Domain))?.Substring(Domain.Length);
                        var username = lines.FirstOrDefault(x => x.StartsWith(Username))?.Substring(Username.Length);

                        importedSessions.Add(new CategoryItem
                        {
                            DisplayName = Path.GetFileNameWithoutExtension(fileName),
                            ConnectionSettings = new ConnectionSettings
                            {
                                Server = server,
                                Port = port,
                                Protocol = Protocol.Rdp,
                                Username = username
                            }
                        });
                    }
                    catch
                    { }
                }
            }
            else
            {
                return null;
            }

            return importedSessions.ToArray();
        }
    }
}
