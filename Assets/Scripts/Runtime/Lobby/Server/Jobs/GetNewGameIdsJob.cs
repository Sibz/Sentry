using Unity.Collections;
using Unity.Jobs;

namespace Sibz.Sentry.Lobby.Server.Jobs
{
    public struct GetNewGameIdsJob : IJob
    {
        public NativeArray<int> Ids;
        [DeallocateOnJobCompletion] public NativeArray<GameIdComponent> GameIds;

        public void Execute()
        {
            for (int i = 0; i < Ids.Length; i++)
            {
                for (int newId = 0; newId < int.MaxValue; newId++)
                {
                    bool inUse = false;
                    for (int j = 0; j < GameIds.Length; j++)
                    {
                        if (GameIds[j].Id != newId)
                        {
                            continue;
                        }

                        inUse = true;
                        break;
                    }

                    if (inUse)
                    {
                        continue;
                    }

                    Ids[i] = newId;
                    break;
                }
            }
        }
    }
}