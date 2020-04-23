using Sibz.Lobby.Requests;
using Unity.Collections;
using Unity.Entities;

namespace Sibz.Lobby.Server.Jobs
{
    public struct DestroyGameJob
    {
        public EntityCommandBuffer.Concurrent CmdBuffer;
        public NativeArray<Entity> GameEntities;
        public NativeArray<GameIdComponent> GameIds;
        public DynamicBuffer<LobbyAclBufferItem> AclBuffer;


        public void Execute(Entity reqEntity, int index, ref DestroyGameRequest rpc, LobbyUser lobbyUser)
        {
            LobbyAclBufferItem currentUserAcl = new LobbyAclBufferItem(lobbyUser.UserId);
            for (int i = 0; i < AclBuffer.Length; i++)
            {
                if (AclBuffer[i].UserId == lobbyUser.UserId)
                {
                    currentUserAcl = AclBuffer[i];
                }
            }

            if (TryGetEntityToDestroy(rpc.Id, out int destroyIndex))
            {
                if (lobbyUser.UserId == GameIds[destroyIndex].UserId && currentUserAcl.DestroyOwnGame
                    || currentUserAcl.DestroyAnyGame)
                {
                    CmdBuffer.DestroyEntity(index, GameEntities[destroyIndex]);
                }
                else
                {
                    // TODO Create unauthorised response
                }
            }

            CmdBuffer.DestroyEntity(index, reqEntity);
        }


        private bool TryGetEntityToDestroy(int id, out int destroyIndex)
        {
            destroyIndex = -1;
            for (int i = 0; i < GameIds.Length; i++)
            {
                if (GameIds[i].Id != id)
                {
                    continue;
                }

                destroyIndex = i;
                return true;
            }

            return false;
        }
    }
}