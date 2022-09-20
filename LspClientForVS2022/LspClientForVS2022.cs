using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Utilities;
using Newtonsoft.Json.Linq;
using StreamJsonRpc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace LspClientForVS2022
{
    public class SemanticTokensData
    {
        public int[] SemTokensBuffer;
        public bool IsTokenDataValid  { get; set; }
        public bool IsNeedSemTokenReq { get; set; }

        public SemanticTokensData(int bufDataLen) 
        {
            SemTokensBuffer = new int[bufDataLen];
            IsTokenDataValid = false;
            IsNeedSemTokenReq = false;
        }

    }

    [ContentType("ore")]
    [Export(typeof(ILanguageClient))]
    public class MyLanguageClient : ILanguageClient, ILanguageClientCustomMessage2
    {
        // Static Factor for ITagger.GetTags()
        private static Dictionary<string, SemanticTokensData> s_dicSemTokensData = new Dictionary<string, SemanticTokensData>();
        public static void GetSemTokensData(string uri, out int[] semTokenData)
        {
            if (!s_dicSemTokensData.ContainsKey(uri)) {
                semTokenData = null;
                return;
            }

            if (!s_dicSemTokensData[uri].IsTokenDataValid) {
                semTokenData = null;
                return;
            }

            semTokenData = s_dicSemTokensData[uri].SemTokensBuffer;
            return;
        }

        // ILanguageClient Implement
        public string Name => "Bar Language Extension";
        public IEnumerable<string> FilesToWatch => null;
        public object InitializationOptions => null;
        public bool ShowNotificationOnInitializeFailed => true;

        public event AsyncEventHandler<EventArgs> StartAsync;
        public event AsyncEventHandler<EventArgs> StopAsync;

        public IEnumerable<string> ConfigurationSections
        {
            get
            {
                yield return "ore";
            }
        }

        public async Task<Connection> ActivateAsync(CancellationToken token)
        {
            await Task.Yield();

            ProcessStartInfo info = new ProcessStartInfo();
            //info.FileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "JsonRpcServer", @"bin\Debug\JsonRpcServer.exe");
            info.FileName = @"D:\git_repos\C#\LanguageServer\JsonRpcServer\bin\Debug\JsonRpcServer.exe";
            Debug.WriteLine(info.FileName);
            info.Arguments = "stdio";
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            info.CreateNoWindow = true;

            Process process = new Process();
            process.StartInfo = info;

            if (process.Start()) {
                return new Connection(process.StandardOutput.BaseStream, process.StandardInput.BaseStream);
            }

            return null;
        }

        public async Task OnLoadedAsync()
        {
            await StartAsync.InvokeAsync(this, EventArgs.Empty);
        }

        public Task OnServerInitializedAsync()
        {
            return Task.CompletedTask;
        }

        public Task<InitializationFailureContext> OnServerInitializeFailedAsync(ILanguageClientInitializationInfo initializationState)
        {
            throw new NotImplementedException();
        }

        // ILanguageClientCustomMessage2 Implement
        public object MiddleLayer => LspClientMiddleLayer.Instance;
        public object CustomMessageTarget => null;
        internal JsonRpc Rpc { get; set; }
        public Task AttachForCustomMessageAsync(JsonRpc rpc)
        {
            this.Rpc = rpc;
            return Task.CompletedTask;
        }
        internal class LspClientMiddleLayer : ILanguageClientMiddleLayer
        {
            internal readonly static LspClientMiddleLayer Instance = new LspClientMiddleLayer();
            private LspClientMiddleLayer() { }

            public bool CanHandle(string methodName)
            {
                bool bLogMessage = (methodName == "window/logMessage");
                bool bSemTokensFull = (methodName == "textDocument/semanticTokens/full");
                bool bDidOpen = (methodName == "textDocument/didOpen");
                bool bDidChange = (methodName == "textDocument/didChange");
                bool bDidClose = (methodName == "textDocument/didClose");

                bool ret = (bLogMessage || bSemTokensFull || bDidOpen || bDidChange || bDidClose);

                return ret;
            }

            private void createDictionary(string uri)
            {
                s_dicSemTokensData.Add(uri, new SemanticTokensData(0));
            }

            private void deleteDictionary(string uri)
            {
                s_dicSemTokensData.Remove(uri);
            }

            public async Task HandleNotificationAsync(string methodName, JToken methodParam, Func<JToken, Task> sendNotification)
            {
                if (methodName == "window/logMessage") {
                    var message = methodParam["message"];

                    Debug.WriteLine(message);

                } else if (methodName == "textDocument/didOpen") {
                    var textDoc = methodParam["textDocument"];
                    var uri = textDoc["uri"];

                    createDictionary((string)uri);
                    s_dicSemTokensData[(string)uri].IsNeedSemTokenReq = true;

                } else if (methodName == "textDocument/didChange") {
                    var textDoc = methodParam["textDocument"];
                    var uri = (string)textDoc["uri"];

                    s_dicSemTokensData[(string)uri].IsNeedSemTokenReq = true;

                } else if (methodName == "textDocument/didChange") {
                    var textDoc = methodParam["textDocument"];
                    var uri = textDoc["uri"];

                    deleteDictionary((string)uri);

                }

                await sendNotification(methodParam);
            }
            public async Task<JToken> HandleRequestAsync(string methodName, JToken methodParam, Func<JToken, Task<JToken>> sendRequest)
            {

                if (methodName == "textDocument/semanticTokens/full") {
                    var textDoc = methodParam["textDocument"];
                    string uri = (string)textDoc["uri"];

                    if (s_dicSemTokensData[uri].IsNeedSemTokenReq) {
                        JToken ret = await sendRequest(methodParam);
                        JArray jarData = (JArray)ret["data"];
                        int[] aiVar = jarData.ToObject<int[]>();

                        s_dicSemTokensData[uri].IsTokenDataValid = false;
                        {
                            Array.Resize(ref s_dicSemTokensData[uri].SemTokensBuffer, aiVar.Count());
                            Array.Copy(aiVar, s_dicSemTokensData[uri].SemTokensBuffer, aiVar.Count());
                        }
                        s_dicSemTokensData[uri].IsTokenDataValid = true;
                        s_dicSemTokensData[uri].IsNeedSemTokenReq = false;

                        return ret;
                    } else {
                        return null;
                    }

                } else {
                    var result = await sendRequest(methodParam);
                    return result;
                }
            }

        }
    }

}
