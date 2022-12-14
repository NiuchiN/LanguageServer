using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using Nerdbank.Streams;
using StreamJsonRpc;
using System.Diagnostics;

namespace JsonRpcServer
{

    class Program
    {
        static async Task<int> Main(string[] args)
        {
            Trace.WriteLine("Hello Trace!");
            if (args.Length > 0 && args[0] == "stdio")
            {
                await RespondToRpcRequestsAsync(FullDuplexStream.Splice(Console.OpenStandardInput(), Console.OpenStandardOutput()), 0);
            }
            else
            {
                await NamedPipeServerAsync();
            }

            Trace.WriteLine("Good Bye Trace!");
            return 0;
        }
        private static async Task NamedPipeServerAsync()
        {
            int clientId = 0;
            while (true)
            {
                await Console.Error.WriteLineAsync("Waiting for client to make a connection...");
                var stream = new NamedPipeServerStream("StreamJsonRpcSamplePipe", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                await stream.WaitForConnectionAsync();
                Task nowait = RespondToRpcRequestsAsync(stream, ++clientId);
            }
        }

        private static async Task RespondToRpcRequestsAsync(Stream stream, int clientId)
        {
            await Console.Error.WriteLineAsync($"Connection request #{clientId} received. Spinning off an async Task to cater to requests.");
            // Attach の第2引数に指定したClassを指定することで、Publicメソッドを公開してくれる。
            // var jsonRpc = JsonRpc.Attach(stream, new Server());

            // Attachは、Streamの設定・ターゲットの追加・StartListeningまでを自動でやってくれる便利関数。細かい設定を入れたければ下記のようにする。
            var jsonRpc = new JsonRpc(stream);
            jsonRpc.AddLocalRpcTarget(new Server(jsonRpc));

            // Trace設定
            var traceListener = new DefaultTraceListener();
            DateTime dt = DateTime.Now;
            string logFileName = dt.ToString($"{dt:yyyymmddhhmmss}") + ".log";

            traceListener.LogFileName = Path.Combine("C:\\Logs", logFileName);
            jsonRpc.TraceSource.Switch.Level = SourceLevels.All;
            jsonRpc.TraceSource.Listeners.Add(traceListener);   
            //jsonRpc.TraceSource.TraceData(TraceEventType.Information, 0, "Log Start");

            // Listening開始
            jsonRpc.StartListening();

            await Console.Error.WriteLineAsync($"JSON-RPC listener attached to #{clientId}. Waiting for requests...");

            // タスクの終了待ち。Clientがパイプをクローズするまでここで待つ。
            await jsonRpc.Completion;
            await Console.Error.WriteLineAsync($"Connection #{clientId} terminated.");
        }
    }
}
