using Unity.Entities;

namespace Sibz.Lobby.Server
{
    public struct LobbyUser : IComponentData
    {
        public int UserId;
    }
}