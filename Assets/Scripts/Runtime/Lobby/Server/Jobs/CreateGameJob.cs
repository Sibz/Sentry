using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.Sentry.Lobby.Server.Jobs
{
    public interface ICreateGameInfoJob<T>

        where T: struct, IRpcCommand
    {
        void Execute(T data);
    }

    public struct CreateGameJob<TCreateGameRequest, TJobPart> :IJobChunk
    where TCreateGameRequest: struct, IRpcCommand
    where TJobPart: struct, ICreateGameInfoJob<TCreateGameRequest>
    {
        [ReadOnly]
        public ArchetypeChunkComponentType<TCreateGameRequest> CreateGameRpcRequests;
        public TJobPart JobPart;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<TCreateGameRequest> createGameRpcRequests = chunk.GetNativeArray(CreateGameRpcRequests);
            for (int i = 0; i < chunk.Count; i++)
            {
                JobPart.Execute(createGameRpcRequests[i]);
            }
        }
    }
}