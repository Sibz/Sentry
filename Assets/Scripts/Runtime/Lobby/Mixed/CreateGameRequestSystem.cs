using Sibz.NetCode;
using Unity.NetCode;

namespace Sibz.Sentry.Lobby
{
    [ClientAndServerSystem]
    [UpdateInWorld(UpdateInWorld.TargetWorld.ClientAndServer)]
    public class CreateGameRequestSystem : RpcCommandRequestSystem<CreateGameRequest>
    {
    }

}