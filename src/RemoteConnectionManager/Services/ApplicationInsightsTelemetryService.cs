using Microsoft.ApplicationInsights;
using System;

namespace RemoteConnectionManager.Services
{
    public class ApplicationInsightsTelemetryService: ITelemetryService
    {
        public void TrackPage(string page)
        {
            var tc = new TelemetryClient();
            tc.TrackPageView(page);
            tc.Flush();
        }

        public void TrackException(Exception exc)
        {
            var tc = new TelemetryClient();
            tc.TrackException(exc);
            tc.Flush();
        }
    }
}
