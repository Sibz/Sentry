using System;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.Sentry
{
    [UpdateInGroup(typeof(LobbyServerWorldSimGroup))]
    public class LobbyServerGoInGameSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            if (!HasSingleton<GhostPrefabCollectionComponent>())
            {
                return;
            }
            Entities.WithNone<SendRpcCommandRequestComponent>().ForEach((Entity reqEnt, ref LobbyServerInGameRequest req, ref ReceiveRpcCommandRequestComponent reqSrc) =>
            {
                PostUpdateCommands.AddComponent<NetworkStreamInGame>(reqSrc.SourceConnection);
                UnityEngine.Debug.Log(
                    $"Server setting connection {EntityManager.GetComponentData<NetworkIdComponent>(reqSrc.SourceConnection).Value} to in game");
                var ghostCollection = GetSingleton<GhostPrefabCollectionComponent>();
                var ghostId = SentryGhostSerializerCollection.FindGhostType<LobbyConnectionGhostSnapshotData>();
                var prefab = EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection.serverPrefabs)[ghostId].Value;
                var player = EntityManager.Instantiate(prefab);

                EntityManager.SetComponentData(player, new LobbyClientConnection() { ConnectionId = EntityManager.GetComponentData<NetworkIdComponent>(reqSrc.SourceConnection).Value});

                PostUpdateCommands.SetComponent(reqSrc.SourceConnection, new CommandTargetComponent {targetEntity = player});

                PostUpdateCommands.DestroyEntity(reqEnt);
            });
        }
    }
}