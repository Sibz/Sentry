using System;
using Sibz.Lobby.Server.Jobs;
using Sibz.NetCode;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[assembly: DisableAutoCreation]

namespace Sibz.Lobby.Server
{
    public class LobbyServer: ServerWorld
    {
        public LobbyServer(Action onListen = null) : base(
            new ServerOptions()
            {
                Address = "0.0.0.0",
                Port = 2165,
                WorldName = "Lobby Server",
                GhostCollectionPrefab = Resources.Load<GameObject>("Collection"),
                SystemAttributes =
                {
                    typeof(LobbyServerSystemAttribute),
                    typeof(LobbyClientAndServerSystemAttribute)
                }
            })
        {
            if (onListen is Action)
            {
                ListenSuccess += onListen;
            }

            WorldCreated += () =>
            {
                World.EntityManager.CreateEntity(
                    World.EntityManager.CreateArchetype(ComponentType.ReadOnly<LobbyAclBufferItem>()));
            };
            CreateWorld();
            Listen();
        }
    }
}