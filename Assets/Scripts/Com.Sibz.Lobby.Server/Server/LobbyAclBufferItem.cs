using Unity.Entities;

namespace Sibz.Lobby.Server
{
    public struct LobbyAclBufferItem : IBufferElementData
    {
        public int UserId;
        public bool DestroyOwnGame;
        public bool DestroyAnyGame;

        public LobbyAclBufferItem(int userId, bool isAdmin = false)
        {
            UserId = userId;
            DestroyOwnGame = true;
            DestroyAnyGame = isAdmin;
        }
    }
}