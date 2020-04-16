using System.Security.Cryptography.X509Certificates;
using Unity.Burst;
using Unity.NetCode;
using Unity.Networking.Transport;

namespace Sibz.Lobby.Requests
{

    [BurstCompile]
    public struct DestroyGameRequest : IRpcCommand
    {
        public int Id;
        public void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteInt(Id);
        }

        public void Deserialize(ref DataStreamReader reader)
        {
            Id = reader.ReadInt();
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
            RpcExecutor.ExecuteCreateRequestComponent<DestroyGameRequest>(ref parameters);
        }
    }
}