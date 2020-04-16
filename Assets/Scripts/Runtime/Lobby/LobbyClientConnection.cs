using Unity.Entities;

namespace Sibz.Sentry
{
    [GenerateAuthoringComponent]
    public struct LobbyClientConnection : IComponentData
    {
        public int ConnectionId;
    }
}