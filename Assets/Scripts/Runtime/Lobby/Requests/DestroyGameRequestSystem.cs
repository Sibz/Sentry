using Sibz.Lobby.Requests;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.Sentry.Lobby.Requests
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(LobbyRpcRequestGroup))]
    public class DestroyGameRequestSystem : RpcCommandRequestSystem<DestroyGameRequest>
    {

    }
}