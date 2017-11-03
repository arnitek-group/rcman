using System;

namespace RemoteConnectionManager.Services
{
    public interface ITelemetryService
    {
        void TrackPage(string page);
        void TrackException(Exception exc);
    }
}
