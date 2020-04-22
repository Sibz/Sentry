using Unity.Entities;

namespace Sibz.Sentry.Lobby
{
    [GenerateAuthoringComponent]
    public struct LobbyClientConnection : IComponentData
    {
        public int ConnectionId;
    }
}