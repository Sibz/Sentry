using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;

namespace Sibz.Sentry
{
    [BurstCompile]
    public struct LobbyServerInGameRequest : IRpcCommand
    {
        [BurstCompile]
        private static void InvokeExecute(ref RpcExecutor.Parameters parameters)
        {
            RpcExecutor.ExecuteCreateRequestComponent<LobbyServerInGameRequest>(ref parameters);
        }

        static PortableFunctionPointer<RpcExecutor.ExecuteDelegate> InvokeExecuteFunctionPointer =
            new PortableFunctionPointer<RpcExecutor.ExecuteDelegate>(InvokeExecute);

        public void Serialize(ref DataStreamWriter writer)
        {
            //throw new System.NotImplementedException();
        }

        public void Deserialize(ref DataStreamReader reader)
        {
            //throw new System.NotImplementedException();
        }

        public PortableFunctionPointer<RpcExecutor.ExecuteDelegate> CompileExecute()
        {
            return InvokeExecuteFunctionPointer;
        }
    }
}