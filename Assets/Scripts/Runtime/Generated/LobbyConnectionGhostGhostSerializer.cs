using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;
using Sibz.Sentry;

public struct LobbyConnectionGhostGhostSerializer : IGhostSerializer<LobbyConnectionGhostSnapshotData>
{
    private ComponentType componentTypeLobbyClientConnection;
    // FIXME: These disable safety since all serializers have an instance of the same type - causing aliasing. Should be fixed in a cleaner way


    public int CalculateImportance(ArchetypeChunk chunk)
    {
        return 1;
    }

    public int SnapshotSize => UnsafeUtility.SizeOf<LobbyConnectionGhostSnapshotData>();
    public void BeginSerialize(ComponentSystemBase system)
    {
        componentTypeLobbyClientConnection = ComponentType.ReadWrite<LobbyClientConnection>();
    }

    public void CopyToSnapshot(ArchetypeChunk chunk, int ent, uint tick, ref LobbyConnectionGhostSnapshotData snapshot, GhostSerializerState serializerState)
    {
        snapshot.tick = tick;
    }
}
