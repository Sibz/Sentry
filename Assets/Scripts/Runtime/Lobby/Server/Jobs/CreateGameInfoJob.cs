using Sibz.Sentry.Components;

namespace Sibz.Sentry.Lobby.Server.Jobs
{
    public struct CreateGameInfoJob : ICreateGameInfoJob<CreateGameRequest, GameInfoComponent>
    {
        public GameInfoComponent ConvertRequestToComponent(CreateGameRequest data, int gameId)
        {
            return new GameInfoComponent
            {
                Id = gameId,
                Name = data.Name,
                SizeX = data.Size.x,
                SizeY = data.Size.y
            };
        }
    }
}