
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using StreamJsonRpc;

namespace JsonRpcServer
{
    internal class LogMsgParams_JsonRpcFormart
    {
        public int type {get; set;}
        public string message {get; set;}
    }
    internal class ErrObj_JsonRpcFormart
    {
        public int code { get; set; }
        public string message { get; set; }
        public object data  { get; set; }
    }
    internal class InitResult_JsonRpcFormart
    {

        public object capabilities {get; set;}
    }

    internal class Request_JsonRpcFormart
    {
        public string jsonrpc { get; set; }

        // id がdefaultの0の時= Jsonメンバから除外され、Notificationとなる。
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int id {get; set;} = 0;
        public string method {get; set;}
        [JsonPropertyName("params")]
        public object params_cls {get; set;} 

    }
    internal class Response_JsonRpcFormart
    {
        public string jsonrpc { get; set; } = "2.0";
        public int id {get; set;}
        public object result {get; set;}
        public ErrObj_JsonRpcFormart error  { get; set; } 
    }
    
    public class Server
    {
        private readonly JsonRpc _rpc;
        private bool _bIsPubDiagnostics = false;

        public Server(JsonRpc rpc)
        {
            _rpc = rpc;
        }

        private void sendMessage(object cMsg)
        {
            string jsonstring = JsonSerializer.Serialize(cMsg);
            _rpc.TraceSource.TraceData(System.Diagnostics.TraceEventType.Verbose, 23, jsonstring);

            Console.WriteLine($"Content-Length: {jsonstring.Length}\r\n");
            Console.WriteLine(jsonstring);
        }

        internal class Cpabilityies_JsonRpcFormart
        {
            public int textDocumentSync {get; set;} = 1;
        }

        [JsonRpcMethod("initialize")]
        public void initialize(int processId, object clientInfo, string locale, string rootPath, string rootUri, ClientCapabilities_LSP capabilities, object trace, object workspaceFolders)
        {
            Console.Error.WriteLine($"Initialized.");

            //var InitializeJson = new Request_JsonRpcFormart
            //{
            //    jsonrpc = "2.0",
            //    method = "window/logMessage",
            //    params_cls = new LogMsgParams_JsonRpcFormart { type = 3, message = "Hello World!!!" }
            //};
            //sendMessage(InitializeJson);
            if (capabilities.textDocument.publishDiagnostics != null) {
                _bIsPubDiagnostics = capabilities.textDocument.publishDiagnostics.relatedInformation;
            }


            var InitializedMsg = new Response_JsonRpcFormart
            {
                result = new InitResult_JsonRpcFormart
                {
                    capabilities = new Cpabilityies_JsonRpcFormart { textDocumentSync = 1 }
                }
            };
            sendMessage(InitializedMsg);

        }

        [JsonRpcMethod("textDocument/didOpen")]
        public void TextDocDidOpen(TextDocumentItem_LSP textDocument)
        {
            Console.Error.WriteLine($"DidOpen.{textDocument.languageId}");   
        }

        [JsonRpcMethod("textDocument/didChange")]
        public void TextDocDidChange(object textDocument, object contentChanges)
        {
            Console.Error.WriteLine($"DidChange.");

        }

    }
}
