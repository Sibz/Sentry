using Sibz.Lobby;
using Sibz.Lobby.Server;

namespace Sibz.Sentry.Lobby
{
    [LobbyServerSystem]
    public class MyCreateGameSystem : CreateGameSystem<CreateGameRequest, CreateGameInfoJob>
    {
        protected override int GetPrefabIndex()
        {
            return SentryGhostSerializerCollection.FindGhostType<GameInfoSnapshotData>();
        }
    }
}