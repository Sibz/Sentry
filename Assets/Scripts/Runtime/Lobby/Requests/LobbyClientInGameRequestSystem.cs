using Unity.Entities;
using Unity.NetCode;

namespace Sibz.Sentry
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(LobbyRpcRequestGroup))]
    public class LobbyClientInGameRequestSystem : RpcCommandRequestSystem<LobbyServerInGameRequest>
    {
    }
}