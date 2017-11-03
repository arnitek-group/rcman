using Microsoft.ApplicationInsights;
using RemoteConnectionManager.Core;
using System;
using System.Collections.Generic;
using System.Management;

namespace RemoteConnectionManager.Services
{
    public class ApplicationInsightsTelemetryService: ITelemetryService
    {
        private readonly string _userId;
        private readonly string _sessionId;

        public ApplicationInsightsTelemetryService()
        {
            var os = new ManagementObject("Win32_OperatingSystem=@");
            var serial = (string) os["SerialNumber"];

            _userId = Security.EncryptText(serial);
            _sessionId = Guid.NewGuid().ToString();
        }

        private TelemetryClient CreateClient()
        {
            var tc = new TelemetryClient();
            tc.Context.User.Id = _userId;
            tc.Context.Session.Id = _sessionId;
            tc.Context.Component.Version = AssemblyInfo.Version;
#if DEBUG
            tc.Context.Properties.Add("Configuration", "DEBUG");
#else
            tc.Context.Properties.Add("Configuration", "RELEASE");
#endif
            // Hide sensitive user data.
            tc.Context.Cloud.RoleInstance = "PC";
            return tc;
        }

        public void TrackPage(string page)
        {
            var tc = CreateClient();
            tc.TrackPageView(page);
            tc.Flush();
        }

        public void TrackEvent(string @event, IDictionary<string, string> properties)
        {
            var tc = CreateClient();
            tc.TrackEvent(@event, properties);
            tc.Flush();
        }

        public void TrackException(Exception exc)
        {
            var tc = CreateClient();
            tc.TrackException(exc);
            tc.Flush();
        }
    }
}
