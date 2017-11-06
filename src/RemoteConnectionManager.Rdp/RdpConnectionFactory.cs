using RemoteConnectionManager.Core;
using RemoteConnectionManager.Core.Connections;
using RemoteConnectionManager.Core.Services;

namespace RemoteConnectionManager.Rdp
{
    public class RdpConnectionFactory : IConnectionFactory
    {
        private readonly ITelemetryService _telemetryService;

        public RdpConnectionFactory(ITelemetryService telemetryService)
        {
            this._telemetryService = telemetryService;
        }

        public Protocol[] Protocols => new[] {Protocol.Rdp};

        public IConnection CreateConnection(ConnectionSettings connectionSettings)
        {
            return new RdpConnection(_telemetryService, connectionSettings);
        }
    }
}
