﻿using Sibz.Sentry.Components;
using Unity.Entities;

namespace Sibz.Sentry.Lobby.Server.Jobs
{
    public struct CreateGameInfoJob : ICreateGameInfoJob<CreateGameRequest>
    {
        private static GameInfoComponent ConvertRequestToComponent(CreateGameRequest data, int gameId)
        {
            return new GameInfoComponent
            {
                Id = gameId,
                Name = data.Name,
                SizeX = data.Size.x,
                SizeY = data.Size.y
            };
        }

        public void SetGameInfoComponent(EntityCommandBuffer.Concurrent commandBuffer, int index, Entity entity, CreateGameRequest data, int gameId)
        {
            commandBuffer.SetComponent(index, entity, ConvertRequestToComponent(data, gameId));
        }
    }
}