using Sibz.Lobby.Requests;
using Sibz.NetCode;
using Unity.Entities;
using Unity.NetCode;

[assembly: DisableAutoCreation]

namespace Sibz.Lobby
{
    [LobbyClientAndServerSystem]
    public class DestroyGameRequestSystem : RpcCommandRequestSystem<DestroyGameRequest>
    {
    }
}