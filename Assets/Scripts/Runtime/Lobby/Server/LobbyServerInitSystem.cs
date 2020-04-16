using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

namespace Sibz.Sentry
{
    [UpdateInGroup(typeof(LobbyServerWorldInitGroup))]
    public class LobbyServerInitSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            NetworkStreamReceiveSystem network = World.GetExistingSystem<NetworkStreamReceiveSystem>();
            NetworkEndPoint ep = NetworkEndPoint.AnyIpv4;
            ep.Port = 21650;
            network.Listen(ep);
            Debug.Log("Lobby Listening on 21650");
            Enabled = false;
        }
    }
}