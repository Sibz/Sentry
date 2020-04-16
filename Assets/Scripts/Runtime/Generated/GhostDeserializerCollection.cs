using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using Unity.NetCode;

public struct SentryGhostDeserializerCollection : IGhostDeserializerCollection
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public string[] CreateSerializerNameList()
    {
        var arr = new string[]
        {
            "LobbyConnectionGhostGhostSerializer",
            "GameInfoGhostSerializer",
        };
        return arr;
    }

    public int Length => 2;
#endif
    public void Initialize(World world)
    {
        var curLobbyConnectionGhostGhostSpawnSystem = world.GetOrCreateSystem<LobbyConnectionGhostGhostSpawnSystem>();
        m_LobbyConnectionGhostSnapshotDataNewGhostIds = curLobbyConnectionGhostGhostSpawnSystem.NewGhostIds;
        m_LobbyConnectionGhostSnapshotDataNewGhosts = curLobbyConnectionGhostGhostSpawnSystem.NewGhosts;
        curLobbyConnectionGhostGhostSpawnSystem.GhostType = 0;
        var curGameInfoGhostSpawnSystem = world.GetOrCreateSystem<GameInfoGhostSpawnSystem>();
        m_GameInfoSnapshotDataNewGhostIds = curGameInfoGhostSpawnSystem.NewGhostIds;
        m_GameInfoSnapshotDataNewGhosts = curGameInfoGhostSpawnSystem.NewGhosts;
        curGameInfoGhostSpawnSystem.GhostType = 1;
    }

    public void BeginDeserialize(JobComponentSystem system)
    {
        m_LobbyConnectionGhostSnapshotDataFromEntity = system.GetBufferFromEntity<LobbyConnectionGhostSnapshotData>();
        m_GameInfoSnapshotDataFromEntity = system.GetBufferFromEntity<GameInfoSnapshotData>();
    }
    public bool Deserialize(int serializer, Entity entity, uint snapshot, uint baseline, uint baseline2, uint baseline3,
        ref DataStreamReader reader, NetworkCompressionModel compressionModel)
    {
        switch (serializer)
        {
            case 0:
                return GhostReceiveSystem<SentryGhostDeserializerCollection>.InvokeDeserialize(m_LobbyConnectionGhostSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, ref reader, compressionModel);
            case 1:
                return GhostReceiveSystem<SentryGhostDeserializerCollection>.InvokeDeserialize(m_GameInfoSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, ref reader, compressionModel);
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }
    public void Spawn(int serializer, int ghostId, uint snapshot, ref DataStreamReader reader,
        NetworkCompressionModel compressionModel)
    {
        switch (serializer)
        {
            case 0:
                m_LobbyConnectionGhostSnapshotDataNewGhostIds.Add(ghostId);
                m_LobbyConnectionGhostSnapshotDataNewGhosts.Add(GhostReceiveSystem<SentryGhostDeserializerCollection>.InvokeSpawn<LobbyConnectionGhostSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            case 1:
                m_GameInfoSnapshotDataNewGhostIds.Add(ghostId);
                m_GameInfoSnapshotDataNewGhosts.Add(GhostReceiveSystem<SentryGhostDeserializerCollection>.InvokeSpawn<GameInfoSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }

    private BufferFromEntity<LobbyConnectionGhostSnapshotData> m_LobbyConnectionGhostSnapshotDataFromEntity;
    private NativeList<int> m_LobbyConnectionGhostSnapshotDataNewGhostIds;
    private NativeList<LobbyConnectionGhostSnapshotData> m_LobbyConnectionGhostSnapshotDataNewGhosts;
    private BufferFromEntity<GameInfoSnapshotData> m_GameInfoSnapshotDataFromEntity;
    private NativeList<int> m_GameInfoSnapshotDataNewGhostIds;
    private NativeList<GameInfoSnapshotData> m_GameInfoSnapshotDataNewGhosts;
}
public struct EnableSentryGhostReceiveSystemComponent : IComponentData
{}
public class SentryGhostReceiveSystem : GhostReceiveSystem<SentryGhostDeserializerCollection>
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<EnableSentryGhostReceiveSystemComponent>();
    }
}
