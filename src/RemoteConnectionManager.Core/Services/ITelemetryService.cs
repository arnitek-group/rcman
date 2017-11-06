using System;
using System.Collections.Generic;

namespace RemoteConnectionManager.Core.Services
{
    public interface ITelemetryService
    {
        void TrackPage(string page);
        void TrackEvent(string @event, IDictionary<string, string> properties);
        void TrackException(Exception exc);
    }
}
