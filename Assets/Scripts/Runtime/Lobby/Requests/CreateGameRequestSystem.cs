using Unity.Entities;
using Unity.NetCode;

namespace Sibz.Sentry.Lobby
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(LobbyRpcRequestGroup))]
    public class CreateGameRequestSystem : RpcCommandRequestSystem<CreateGameRequest>
    {
    }
}