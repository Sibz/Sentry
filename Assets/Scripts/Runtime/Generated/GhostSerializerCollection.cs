using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using Unity.NetCode;

public struct SentryGhostSerializerCollection : IGhostSerializerCollection
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
    public static int FindGhostType<T>()
        where T : struct, ISnapshotData<T>
    {
        if (typeof(T) == typeof(LobbyConnectionGhostSnapshotData))
            return 0;
        if (typeof(T) == typeof(GameInfoSnapshotData))
            return 1;
        return -1;
    }

    public void BeginSerialize(ComponentSystemBase system)
    {
        m_LobbyConnectionGhostGhostSerializer.BeginSerialize(system);
        m_GameInfoGhostSerializer.BeginSerialize(system);
    }

    public int CalculateImportance(int serializer, ArchetypeChunk chunk)
    {
        switch (serializer)
        {
            case 0:
                return m_LobbyConnectionGhostGhostSerializer.CalculateImportance(chunk);
            case 1:
                return m_GameInfoGhostSerializer.CalculateImportance(chunk);
        }

        throw new ArgumentException("Invalid serializer type");
    }

    public int GetSnapshotSize(int serializer)
    {
        switch (serializer)
        {
            case 0:
                return m_LobbyConnectionGhostGhostSerializer.SnapshotSize;
            case 1:
                return m_GameInfoGhostSerializer.SnapshotSize;
        }

        throw new ArgumentException("Invalid serializer type");
    }

    public int Serialize(ref DataStreamWriter dataStream, SerializeData data)
    {
        switch (data.ghostType)
        {
            case 0:
            {
                return GhostSendSystem<SentryGhostSerializerCollection>.InvokeSerialize<LobbyConnectionGhostGhostSerializer, LobbyConnectionGhostSnapshotData>(m_LobbyConnectionGhostGhostSerializer, ref dataStream, data);
            }
            case 1:
            {
                return GhostSendSystem<SentryGhostSerializerCollection>.InvokeSerialize<GameInfoGhostSerializer, GameInfoSnapshotData>(m_GameInfoGhostSerializer, ref dataStream, data);
            }
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }
    private LobbyConnectionGhostGhostSerializer m_LobbyConnectionGhostGhostSerializer;
    private GameInfoGhostSerializer m_GameInfoGhostSerializer;
}

public struct EnableSentryGhostSendSystemComponent : IComponentData
{}
public class SentryGhostSendSystem : GhostSendSystem<SentryGhostSerializerCollection>
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<EnableSentryGhostSendSystemComponent>();
    }
}
