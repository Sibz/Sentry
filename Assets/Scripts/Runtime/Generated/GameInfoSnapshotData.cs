using Unity.Networking.Transport;
using Unity.NetCode;
using Unity.Mathematics;
using Unity.Collections;

public struct GameInfoSnapshotData : ISnapshotData<GameInfoSnapshotData>
{
    public uint tick;
    private NativeString64 GameInfoComponentName;
    private int GameInfoComponentSizeX;
    private int GameInfoComponentSizeY;
    private int GameInfoComponentId;
    uint changeMask0;

    public uint Tick => tick;
    public NativeString64 GetGameInfoComponentName(GhostDeserializerState deserializerState)
    {
        return GameInfoComponentName;
    }
    public NativeString64 GetGameInfoComponentName()
    {
        return GameInfoComponentName;
    }
    public void SetGameInfoComponentName(NativeString64 val, GhostSerializerState serializerState)
    {
        GameInfoComponentName = val;
    }
    public void SetGameInfoComponentName(NativeString64 val)
    {
        GameInfoComponentName = val;
    }
    public int GetGameInfoComponentSizeX(GhostDeserializerState deserializerState)
    {
        return (int)GameInfoComponentSizeX;
    }
    public int GetGameInfoComponentSizeX()
    {
        return (int)GameInfoComponentSizeX;
    }
    public void SetGameInfoComponentSizeX(int val, GhostSerializerState serializerState)
    {
        GameInfoComponentSizeX = (int)val;
    }
    public void SetGameInfoComponentSizeX(int val)
    {
        GameInfoComponentSizeX = (int)val;
    }
    public int GetGameInfoComponentSizeY(GhostDeserializerState deserializerState)
    {
        return (int)GameInfoComponentSizeY;
    }
    public int GetGameInfoComponentSizeY()
    {
        return (int)GameInfoComponentSizeY;
    }
    public void SetGameInfoComponentSizeY(int val, GhostSerializerState serializerState)
    {
        GameInfoComponentSizeY = (int)val;
    }
    public void SetGameInfoComponentSizeY(int val)
    {
        GameInfoComponentSizeY = (int)val;
    }
    public int GetGameInfoComponentId(GhostDeserializerState deserializerState)
    {
        return (int)GameInfoComponentId;
    }
    public int GetGameInfoComponentId()
    {
        return (int)GameInfoComponentId;
    }
    public void SetGameInfoComponentId(int val, GhostSerializerState serializerState)
    {
        GameInfoComponentId = (int)val;
    }
    public void SetGameInfoComponentId(int val)
    {
        GameInfoComponentId = (int)val;
    }

    public void PredictDelta(uint tick, ref GameInfoSnapshotData baseline1, ref GameInfoSnapshotData baseline2)
    {
        var predictor = new GhostDeltaPredictor(tick, this.tick, baseline1.tick, baseline2.tick);
        GameInfoComponentSizeX = predictor.PredictInt(GameInfoComponentSizeX, baseline1.GameInfoComponentSizeX, baseline2.GameInfoComponentSizeX);
        GameInfoComponentSizeY = predictor.PredictInt(GameInfoComponentSizeY, baseline1.GameInfoComponentSizeY, baseline2.GameInfoComponentSizeY);
        GameInfoComponentId = predictor.PredictInt(GameInfoComponentId, baseline1.GameInfoComponentId, baseline2.GameInfoComponentId);
    }

    public void Serialize(int networkId, ref GameInfoSnapshotData baseline, ref DataStreamWriter writer, NetworkCompressionModel compressionModel)
    {
        changeMask0 = GameInfoComponentName.Equals(baseline.GameInfoComponentName) ? 0 : 1u;
        changeMask0 |= (GameInfoComponentSizeX != baseline.GameInfoComponentSizeX) ? (1u<<1) : 0;
        changeMask0 |= (GameInfoComponentSizeY != baseline.GameInfoComponentSizeY) ? (1u<<2) : 0;
        changeMask0 |= (GameInfoComponentId != baseline.GameInfoComponentId) ? (1u<<3) : 0;
        writer.WritePackedUIntDelta(changeMask0, baseline.changeMask0, compressionModel);
        if ((changeMask0 & (1 << 0)) != 0)
            writer.WritePackedStringDelta(GameInfoComponentName, baseline.GameInfoComponentName, compressionModel);
        if ((changeMask0 & (1 << 1)) != 0)
            writer.WritePackedIntDelta(GameInfoComponentSizeX, baseline.GameInfoComponentSizeX, compressionModel);
        if ((changeMask0 & (1 << 2)) != 0)
            writer.WritePackedIntDelta(GameInfoComponentSizeY, baseline.GameInfoComponentSizeY, compressionModel);
        if ((changeMask0 & (1 << 3)) != 0)
            writer.WritePackedIntDelta(GameInfoComponentId, baseline.GameInfoComponentId, compressionModel);
    }

    public void Deserialize(uint tick, ref GameInfoSnapshotData baseline, ref DataStreamReader reader,
        NetworkCompressionModel compressionModel)
    {
        this.tick = tick;
        changeMask0 = reader.ReadPackedUIntDelta(baseline.changeMask0, compressionModel);
        if ((changeMask0 & (1 << 0)) != 0)
            GameInfoComponentName = reader.ReadPackedStringDelta(baseline.GameInfoComponentName, compressionModel);
        else
            GameInfoComponentName = baseline.GameInfoComponentName;
        if ((changeMask0 & (1 << 1)) != 0)
            GameInfoComponentSizeX = reader.ReadPackedIntDelta(baseline.GameInfoComponentSizeX, compressionModel);
        else
            GameInfoComponentSizeX = baseline.GameInfoComponentSizeX;
        if ((changeMask0 & (1 << 2)) != 0)
            GameInfoComponentSizeY = reader.ReadPackedIntDelta(baseline.GameInfoComponentSizeY, compressionModel);
        else
            GameInfoComponentSizeY = baseline.GameInfoComponentSizeY;
        if ((changeMask0 & (1 << 3)) != 0)
            GameInfoComponentId = reader.ReadPackedIntDelta(baseline.GameInfoComponentId, compressionModel);
        else
            GameInfoComponentId = baseline.GameInfoComponentId;
    }
    public void Interpolate(ref GameInfoSnapshotData target, float factor)
    {
    }
}
