using Microsoft.ApplicationInsights;
using Microsoft.VisualBasic.Devices;
using RemoteConnectionManager.Core;
using RemoteConnectionManager.Core.Services;
using System;
using System.Collections.Generic;
using System.Management;
using System.Threading.Tasks;

namespace RemoteConnectionManager.Services
{
    public class ApplicationInsightsTelemetryService: ITelemetryService
    {
        private readonly string _os;
        private readonly string _userId;
        private readonly string _sessionId;

        public ApplicationInsightsTelemetryService()
        {
            _os = new ComputerInfo().OSFullName;

            try
            {
                var os = new ManagementObject("Win32_OperatingSystem=@");
                var serial = (string)os["SerialNumber"];
                _userId = Security.EncryptText(serial);
            }
            catch
            {
                _userId = "Generic";
            }

            _sessionId = Guid.NewGuid().ToString();
        }

        private void FlushAsync(Action<TelemetryClient> track)
        {
            Task.Factory.StartNew(() =>
            {
                var tc = new TelemetryClient();
                tc.Context.Device.OperatingSystem = _os;
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

                track(tc);
#if DEBUG
#else
                tc.Flush();
#endif
            });
        }

        public void TrackPage(string page)
        {
            FlushAsync(tc =>
            {
                tc.TrackPageView(page);
            });
        }

        public void TrackEvent(string @event, IDictionary<string, string> properties)
        {
            FlushAsync(tc =>
            {
                tc.TrackEvent(@event, properties);
            });
        }

        public void TrackException(Exception exc)
        {
            FlushAsync(tc =>
            {
                tc.TrackException(exc);
            });
        }
    }
}
