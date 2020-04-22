using System;
using System.Collections.Generic;
using Sibz.Lobby.Requests;
using Sibz.NetCode;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[assembly: DisableAutoCreation]

namespace Sibz.Lobby.Client
{
    public abstract class LobbyClient<TGameInfoComponent, TCreateGameRequest> : ClientWorld
    where TGameInfoComponent: struct, IComponentData
    where TCreateGameRequest: struct, IRpcCommand
    {
        public class RefreshGameListSystem : RefreshGameListSystem<TGameInfoComponent>
        {
        }
        private RefreshGameListSystem<TGameInfoComponent> refreshGameListSystem;
        public TGameInfoComponent[] Games => refreshGameListSystem.GameInfoComponents.ToArray();
        public Action GameListUpdated;
        public LobbyClient() : base(new ClientOptions()
        {
            Address = "127.0.0.1",
            Port = 2165,
            CreateWorldOnInstantiate = false,
            WorldName = "LobbyClient",
            GhostCollectionPrefab = Resources.Load<GameObject>("Collection"),
            Systems = new List<Type>()
            {
                typeof(RefreshGameListSystem)
            },
            SystemAttributes =
            {
                typeof(LobbyClientSystemAttribute),
                typeof(LobbyClientAndServerSystemAttribute)
            }
        })
        {
            WorldCreated += () =>
            {
                refreshGameListSystem = World.GetExistingSystem<RefreshGameListSystem<TGameInfoComponent>>();
                refreshGameListSystem.GameListUpdated += () => GameListUpdated?.Invoke();
            };
        }

        public void CreateNewGame(TCreateGameRequest request)
        {
            World.CreateRpcRequest(request);
        }

        public void DestroyGame(int gameId)
        {
            World.CreateRpcRequest(new DestroyGameRequest { Id = gameId });
        }
    }
}