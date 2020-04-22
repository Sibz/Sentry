using System;
using System.Collections.Generic;
using Sibz.Lobby.Requests;
using Sibz.NetCode;
using Unity.Entities;
using UnityEngine;

[assembly: DisableAutoCreation]

namespace Sibz.Lobby.Client
{
    public abstract class LobbyClient<TGameInfoComponent> : ClientWorld
    where TGameInfoComponent: struct, IComponentData
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

        //public abstract void CreateNewGame(string name);
        /*{
            World.CreateRpcRequest(new CreateGameRequest { Name = name, Size = new int2(3, 3) });
        }*/

        public void DestroyGame(int gameId)
        {
            World.CreateRpcRequest(new DestroyGameRequest { Id = gameId });
        }
    }
}