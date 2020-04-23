using Unity.Entities;

namespace Sibz.Lobby.Server
{
    public struct GameIdComponent : IComponentData
    {
        public int Id;
        public int UserId;
    }
}