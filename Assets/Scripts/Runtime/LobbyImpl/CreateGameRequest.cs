using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Networking.Transport;

namespace Sibz.Sentry.Lobby
{
    [BurstCompile]
    public struct CreateGameRequest : IRpcCommand
    {
        public NativeString64 Name;
        public int2 Size;

        public void Serialize(ref DataStreamWriter writer)
        {
            //writer.WriteInt(1);
            writer.WriteString(Name);
            writer.WriteInt(Size.x);
            writer.WriteInt(Size.y);
        }

        public void Deserialize(ref DataStreamReader reader)
        {
            Name = reader.ReadString();
            Size.x = reader.ReadInt();
            Size.y = reader.ReadInt();
            //reader.ReadInt();
        }

        static PortableFunctionPointer<RpcExecutor.ExecuteDelegate> InvokeExecuteFunctionPointer =
            new PortableFunctionPointer<RpcExecutor.ExecuteDelegate>(InvokeExecute);

        public PortableFunctionPointer<RpcExecutor.ExecuteDelegate> CompileExecute()
        {
            return InvokeExecuteFunctionPointer;
        }

        [BurstCompile]
        private static void InvokeExecute(ref RpcExecutor.Parameters parameters)
        {
            RpcExecutor.ExecuteCreateRequestComponent<CreateGameRequest>(ref parameters);
        }

    }
}