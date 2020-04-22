using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;

namespace Sibz.Sentry.Lobby.Server.Jobs
{
    [BurstCompile]
    public struct CreateGameJob<TCreateGameRequest, TCreateGameInfoJob> : IJobParallelFor
        where TCreateGameRequest : struct, IRpcCommand
        where TCreateGameInfoJob : struct, ICreateGameInfoJob<TCreateGameRequest>
    {
        [ReadOnly][DeallocateOnJobCompletion] public NativeArray<TCreateGameRequest> CreateGameRpcRequests;
        [ReadOnly][DeallocateOnJobCompletion] public NativeArray<int> NewGameIds;
        public EntityCommandBuffer.Concurrent CommandBuffer;
        public TCreateGameInfoJob CreateGameInfoJob;
        public Entity Prefab;

        public void Execute(int index)
        {
            int gameId = NewGameIds[index];
            TCreateGameRequest data = CreateGameRpcRequests[index];
            GameIdComponent gameIdComponent = new GameIdComponent { Id = gameId };

            if (CreateGameInfoJob.ValidateRequest(data) != 0)
            {
                return;
            }
            Entity newGameEntity = CommandBuffer.Instantiate(index, Prefab);
            CreateGameInfoJob.SetGameInfoComponent(CommandBuffer, index, newGameEntity, data, gameId);
            CommandBuffer.SetComponent(index, newGameEntity, gameIdComponent);
        }
    }
}