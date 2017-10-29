
namespace RemoteConnectionManager.Core
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
