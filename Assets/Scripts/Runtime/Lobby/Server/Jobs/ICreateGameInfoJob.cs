using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.Sentry.Lobby.Server.Jobs
{
    public interface ICreateGameInfoJob<in TCreateGameRequest>
        where TCreateGameRequest : struct, IRpcCommand
    {

        void SetGameInfoComponent(EntityCommandBuffer.Concurrent commandBuffer, int index, Entity entity,
            TCreateGameRequest data, int gameId);

        int ValidateRequest(TCreateGameRequest data);
    }
}