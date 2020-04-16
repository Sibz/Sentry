using Sibz.Lobby.Requests;
using Sibz.Sentry.Components;
using Sibz.Sentry.Lobby.Requests;
using Unity.Collections;
using Unity.Entities;

namespace Sibz.Sentry.Lobby.Server.Jobs
{
    public struct DestroyGameJob
    {
        /*
        public struct Data
        {
            public EntityCommandBuffer.Concurrent CmdBuffer;
            public NativeArray<Entity> GameEntities;
            public NativeArray<GameInfoComponent> GameInfos;
        }

        public static void Execute(Data data, Entity reqEntity, int index, ref DestroyGameRequest rpc)
        {
            if (TryGetEntityToDestroy(data.GameEntities, data.GameInfos, rpc.Id, out Entity destroyEntity))
            {
                data.CmdBuffer.DestroyEntity(index, destroyEntity);
            }

            data.CmdBuffer.DestroyEntity(index, reqEntity);
        }
        */


            public EntityCommandBuffer.Concurrent CmdBuffer;
            public NativeArray<Entity> GameEntities;
            public NativeArray<GameInfoComponent> GameInfos;


        public void  Execute(Entity reqEntity, int index, ref DestroyGameRequest rpc)
        {
            if (TryGetEntityToDestroy(rpc.Id, out Entity destroyEntity))
            {
                CmdBuffer.DestroyEntity(index, destroyEntity);
            }

            CmdBuffer.DestroyEntity(index, reqEntity);
        }


        private bool TryGetEntityToDestroy(int id, out Entity entity)
        {
            entity = Entity.Null;
            for (int i = 0; i < GameInfos.Length; i++)
            {
                if (GameInfos[i].Id != id)
                {
                    continue;
                }

                entity = GameEntities[i];
                return true;
            }

            return false;
        }
    }
}