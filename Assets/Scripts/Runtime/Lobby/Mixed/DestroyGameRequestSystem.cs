using Sibz.Lobby.Requests;
using Sibz.NetCode;
using Unity.NetCode;

namespace Sibz.Sentry.Lobby.Requests
{
    [ClientAndServerSystem]
    [UpdateInWorld(UpdateInWorld.TargetWorld.ClientAndServer)]
    public class DestroyGameRequestSystem : RpcCommandRequestSystem<DestroyGameRequest>
    {

    }
}