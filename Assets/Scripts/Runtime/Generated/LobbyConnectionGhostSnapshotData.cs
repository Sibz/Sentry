using Unity.Networking.Transport;
using Unity.NetCode;
using Unity.Mathematics;

public struct LobbyConnectionGhostSnapshotData : ISnapshotData<LobbyConnectionGhostSnapshotData>
{
    public uint tick;

    public uint Tick => tick;

    public void PredictDelta(uint tick, ref LobbyConnectionGhostSnapshotData baseline1, ref LobbyConnectionGhostSnapshotData baseline2)
    {
        var predictor = new GhostDeltaPredictor(tick, this.tick, baseline1.tick, baseline2.tick);
    }

    public void Serialize(int networkId, ref LobbyConnectionGhostSnapshotData baseline, ref DataStreamWriter writer, NetworkCompressionModel compressionModel)
    {
    }

    public void Deserialize(uint tick, ref LobbyConnectionGhostSnapshotData baseline, ref DataStreamReader reader,
        NetworkCompressionModel compressionModel)
    {
        this.tick = tick;
    }
    public void Interpolate(ref LobbyConnectionGhostSnapshotData target, float factor)
    {
    }
}
