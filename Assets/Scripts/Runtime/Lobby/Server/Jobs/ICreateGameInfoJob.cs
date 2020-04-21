using Unity.Entities;
using Unity.NetCode;

namespace Sibz.Sentry.Lobby.Server.Jobs
{
    public interface ICreateGameInfoJob<in TCreateGameRequest, out TGameInfoComponent>
        where TCreateGameRequest : struct, IRpcCommand
        where TGameInfoComponent : struct, IComponentData
    {
        TGameInfoComponent ConvertRequestToComponent(TCreateGameRequest data, int gameId);
    }
}