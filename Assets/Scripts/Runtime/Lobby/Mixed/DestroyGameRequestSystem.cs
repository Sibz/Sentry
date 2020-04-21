using Sibz.Lobby.Requests;
using Sibz.NetCode;
using Unity.NetCode;

namespace Sibz.Sentry.Lobby.Requests
{
    [ClientAndServerSystem]
    public class DestroyGameRequestSystem : RpcCommandRequestSystem<DestroyGameRequest>
    {

    }
}