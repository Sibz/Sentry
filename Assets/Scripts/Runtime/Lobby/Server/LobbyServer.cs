using System;
using Sibz.NetCode;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Sibz.Sentry.Lobby.Server
{
    public class LobbyServer : ServerWorld
    {
        public LobbyServer(Action onListen = null) : base(
            new ServerOptions()
            {
                Address = "0.0.0.0",
                Port = 2165,
                WorldName = "Lobby Server",
                GhostCollectionPrefab = Resources.Load<GameObject>("Collection")
            })
        {
            if (onListen is Action)
            {
                ListenSuccess += onListen;
            }

            ClientConnected += entity =>
            {
                var ghostCollection = World.EntityManager.CreateEntityQuery(typeof(GhostPrefabCollectionComponent)).GetSingleton<GhostPrefabCollectionComponent>();
                var ghostId = SentryGhostSerializerCollection.FindGhostType<LobbyConnectionGhostSnapshotData>();
                var prefab = World.EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection.serverPrefabs)[ghostId].Value;
                var player = World.EntityManager.Instantiate(prefab);
                World.EntityManager.SetComponentData(player, new LobbyClientConnection() { ConnectionId = World.EntityManager.GetComponentData<NetworkIdComponent>(entity).Value});

                World.EntityManager.SetComponentData(entity, new CommandTargetComponent {targetEntity = player});
            };
            CreateWorld();
            Listen();
        }
    }
}