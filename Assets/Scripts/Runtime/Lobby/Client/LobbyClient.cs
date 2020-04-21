using System;
using Sibz.Lobby.Requests;
using Sibz.NetCode;
using Sibz.Sentry.Components;
using Sibz.Sentry.Lobby;
using Unity.Mathematics;
using UnityEngine;

namespace Sibz.Sentry.Client
{
    public class LobbyClient : ClientWorld
    {
        private RefreshGameListSystem refreshGameListSystem;
        public GameInfoComponent[] Games => refreshGameListSystem.GameInfoComponents.ToArray();
        public Action GameListUpdated;
        public LobbyClient() : base(new ClientOptions()
        {
            Address = "127.0.0.1",
            Port = 2165,
            CreateWorldOnInstantiate = false,
            WorldName = "LobbyClient",
            GhostCollectionPrefab = Resources.Load<GameObject>("Collection")
        })
        {
            WorldCreated += () =>
            {
                refreshGameListSystem = World.GetExistingSystem<RefreshGameListSystem>();
                refreshGameListSystem.GameListUpdated += () => GameListUpdated?.Invoke();
            };
        }

        public void CreateNewGame(string name)
        {
            World.CreateRpcRequest(new CreateGameRequest { Name = name, Size = new int2(3, 3) });
        }

        public void DestroyGame(int gameId)
        {
            World.CreateRpcRequest(new DestroyGameRequest { Id = gameId });
        }
    }
}