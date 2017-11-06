
namespace RemoteConnectionManager.Core.Connections
{
    public enum DisconnectReason
    {
        ApplicationExit,
        ServerNotFound,
        ConnectionEnded,
        ConnectionTimedOut,
        ConnectionTerminated,
        KickedOut
    }
}
