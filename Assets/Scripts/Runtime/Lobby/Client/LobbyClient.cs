using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sibz.Lobby.Requests;
using Sibz.RpcRequestConverter;
using Sibz.Sentry.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine.UIElements;

namespace Sibz.Sentry.Lobby.Client
{
    public class LobbyClient
    {
        private World lobbyClientWorld;

        private VisualTreeAsset gameItemTemplate;


        public bool IsConnected => !(lobbyClientWorld is null) &&
                                   lobbyClientWorld.GetExistingSystem<ConnectSystem>().IsConnected;

        public void CreateLobbyWorld()
        {
            if (!(lobbyClientWorld is null))
            {
                return;
            }

            lobbyClientWorld =
                ClientServerBootstrap.CreateClientWorld(World.DefaultGameObjectInjectionWorld, "Lobby");
            lobbyClientWorld.GetExistingSystem<ClientInitializationSystemGroup>()
                .AddSystemToUpdateList(lobbyClientWorld.CreateSystem<InitialisationGroup>());
            lobbyClientWorld.GetExistingSystem<ClientSimulationSystemGroup>()
                .AddSystemToUpdateList(lobbyClientWorld.CreateSystem<SimulationGroup>());
            lobbyClientWorld.GetExistingSystem<RpcCommandRequestSystemGroup>()
                .AddSystemToUpdateList(lobbyClientWorld.CreateSystem<LobbyRpcRequestGroup>());
            ConvertToRpcRequestSystem.CreateInWorld(lobbyClientWorld);
        }

        public void CreateNewGame(string name, int2 size = default)
        {
            if (size.Equals(default))
            {
                size.x = 4;
                size.y = 4;
            }

            CreateRpcRequest(new CreateGameRequest
            {
                Name = name,
                Size = size
            });
        }

        public void DestroyGame(int id)
        {
            CreateRpcRequest(new DestroyGameRequest{Id = id});
        }

        public void ConnectToServerLobby()
        {
            lobbyClientWorld.EntityManager.CreateEntity(typeof(Connect));
        }

        private void Disconnect()
        {
            lobbyClientWorld.EntityManager.CreateEntity(typeof(Disconnect));
        }

        public async Task DisconnectAsync(bool killWorld = false)
        {
            Disconnect();
            var q = lobbyClientWorld.EntityManager.CreateEntityQuery(typeof(NetworkStreamConnection));
            if (killWorld)
            {
                var t = new Task(() =>
                {
                    //Thread.Sleep(250);
                    int maxWait = 40;
                    while (q.CalculateEntityCount() > 0 && maxWait > 0)
                    {
                        maxWait--;
                        Thread.Sleep(50);
                    }
                });
                t.Start();
                await t;
                KillWorld();
            }
        }

        public void KillWorld()
        {
            lobbyClientWorld?.Dispose();
            lobbyClientWorld = null;
        }

        public IEnumerable<GameInfoComponent> GetItems()
        {
            var na = lobbyClientWorld.EntityManager
                .CreateEntityQuery(typeof(GameInfoComponent))
                .ToComponentDataArrayAsync<GameInfoComponent>(Allocator.Persistent, out JobHandle handle);
            handle.Complete();

            var arr = na.ToArray();
            na.Dispose();
            return arr;
        }

        private void CreateRpcRequest<T>(T rpcCommand)
            where T : struct, IRpcCommand => ConvertToRpcRequestSystem.CreateRpcRequest(lobbyClientWorld, rpcCommand);

    }
}