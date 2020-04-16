using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;
using Sibz.Sentry.Components;

public struct GameInfoGhostSerializer : IGhostSerializer<GameInfoSnapshotData>
{
    private ComponentType componentTypeGameInfoComponent;
    // FIXME: These disable safety since all serializers have an instance of the same type - causing aliasing. Should be fixed in a cleaner way
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<GameInfoComponent> ghostGameInfoComponentType;


    public int CalculateImportance(ArchetypeChunk chunk)
    {
        return 1;
    }

    public int SnapshotSize => UnsafeUtility.SizeOf<GameInfoSnapshotData>();
    public void BeginSerialize(ComponentSystemBase system)
    {
        componentTypeGameInfoComponent = ComponentType.ReadWrite<GameInfoComponent>();
        ghostGameInfoComponentType = system.GetArchetypeChunkComponentType<GameInfoComponent>(true);
    }

    public void CopyToSnapshot(ArchetypeChunk chunk, int ent, uint tick, ref GameInfoSnapshotData snapshot, GhostSerializerState serializerState)
    {
        snapshot.tick = tick;
        var chunkDataGameInfoComponent = chunk.GetNativeArray(ghostGameInfoComponentType);
        snapshot.SetGameInfoComponentName(chunkDataGameInfoComponent[ent].Name, serializerState);
        snapshot.SetGameInfoComponentSizeX(chunkDataGameInfoComponent[ent].SizeX, serializerState);
        snapshot.SetGameInfoComponentSizeY(chunkDataGameInfoComponent[ent].SizeY, serializerState);
        snapshot.SetGameInfoComponentId(chunkDataGameInfoComponent[ent].Id, serializerState);
    }
}
