
using System;
using System.Collections.Generic;
using StreamJsonRpc;

namespace JsonRpcServer
{
    public class Server
    {
        private readonly JsonRpc _rpc;
        private bool _bIsPubDiagnostics = false;
        private List<Diagnostic_LSP> _lstDiagnostics = new List<Diagnostic_LSP>();

        public Server(JsonRpc rpc)
        {
            _rpc = rpc;
        }

        private void sendMessage(string strMethod, object objParams)
        {
            _ = _rpc.NotifyWithParameterObjectAsync(strMethod, objParams);
        }

        private void sendLogMessage(MessageType eType, string strMsg)
        {
            var LogMessageParams = new LogMessageParams_LSP { type = eType, message = strMsg };
            sendMessage("window/logMessage", LogMessageParams);
        }

        private void sendDiagnostics(string uri)
        {
            if (_bIsPubDiagnostics)
            {
                var DiagnosticJson = new PublishDiagnosticsParams_LSP
                {
                    uri = uri,
                    diagnostics = new List<Diagnostic_LSP>(_lstDiagnostics)
                };

                sendMessage("textDocument/publishDiagnostics", DiagnosticJson);
            }

        }

        private void compile(string uri)
        {
            Diagnostic_LSP[] diagnosticLSP = new Diagnostic_LSP[3];

            diagnosticLSP[0] = new Diagnostic_LSP
            {
                message = "test NG1",
                range = new Range(0, 1, 0, 3),
                severity = DiagnosticSeverity.Error
            };

            diagnosticLSP[1] = new Diagnostic_LSP
            {
                message = "test NG2",
                range = new Range(0, 4, 0, 5),
                severity = DiagnosticSeverity.Error
            };

            diagnosticLSP[2] = new Diagnostic_LSP
            {
                message = "test NG3",
                range = new Range(1, 0, 1, 5),
                severity = DiagnosticSeverity.Error
            };
            _lstDiagnostics.Add(diagnosticLSP[0]);
            _lstDiagnostics.Add(diagnosticLSP[1]);
            _lstDiagnostics.Add(diagnosticLSP[2]);

            sendDiagnostics(uri); 
        }

        [JsonRpcMethod("initialize")]
        public object initialize(int processId, object clientInfo, string locale, string rootPath, string rootUri, ClientCapabilities_LSP capabilities, object trace, object workspaceFolders)
        {
            if (capabilities.textDocument.publishDiagnostics != null) {
                _bIsPubDiagnostics = capabilities.textDocument.publishDiagnostics.relatedInformation;
            }

            sendLogMessage(MessageType.Info, "Initialized!");

            var initializedResult = new ServerCapabilities_LSP
            {
                capabilities = new ServerCpabilities
                    {
                        textDocumentSync = new TextDocumentSyncOptions { openClose = true, change = TextDocumentSyncKind.Full }
                    }
            };
            return initializedResult;
        }

        [JsonRpcMethod("textDocument/didOpen")]
        public void TextDocDidOpen(TextDocumentItem_LSP textDocument)
        {
            compile(textDocument.uri);
        }

        [JsonRpcMethod("textDocument/didChange")]
        public void TextDocDidChange(object textDocument, object contentChanges)
        {
            Console.Error.WriteLine($"DidChange.");

        }

    }
}
