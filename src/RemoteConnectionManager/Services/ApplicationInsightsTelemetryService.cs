using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;

namespace RemoteConnectionManager.Services
{
    public class ApplicationInsightsTelemetryService: ITelemetryService
    {
        private readonly string _sessionId;
        public ApplicationInsightsTelemetryService()
        {
            _sessionId = Guid.NewGuid().ToString();
        }

        private TelemetryClient CreateClient()
        {
            var tc = new TelemetryClient();
            tc.Context.Session.Id = _sessionId;
            tc.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
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
