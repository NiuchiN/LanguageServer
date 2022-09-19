
using System;
using System.Collections.Generic;
using StreamJsonRpc;
using Shared;

namespace JsonRpcServer
{


    public class Server
    {
        private readonly JsonRpc _rpc;
        private InstructionHint _instSignature;
        private InstructionHashList _instHashList;
        private bool _bIsPubDiagnostics = false;
        private string _LSPServerVer;
        private List<Diagnostic_LSP> _lstDiagnostics = new List<Diagnostic_LSP>();
        private Dictionary<string, int> _dicSemTokensTypes = new Dictionary<string, int>();
        private Dictionary<string, int> _dicSemTokensModifiers = new Dictionary<string, int>();
        private Dictionary<string, Lexer> _dicLexer = new Dictionary<string, Lexer>();
        private bool[] _bIsTokenTypes = new bool[(int)SemTokensTypeIdx.MaxNum];
        private bool[] _bIsTokenModifiers = new bool[(int)SemTokensModifyIdx.MaxNum];

        public Server(JsonRpc rpc, string signatureInputPath, string lSPServerVer)
        {
            _rpc = rpc;
            _LSPServerVer = lSPServerVer;

            // SemanticToken用のディクショナリを初期化
            foreach (SemTokensTypeIdx eTypesVal in Enum.GetValues(typeof(SemTokensTypeIdx))) {
                if (eTypesVal == SemTokensTypeIdx.MaxNum) break;
                _dicSemTokensTypes.Add(eTypesVal.GetStr(), (int)eTypesVal);
            }
            foreach (SemTokensModifyIdx eModifyVal in Enum.GetValues(typeof(SemTokensModifyIdx))) {
                if (eModifyVal == SemTokensModifyIdx.MaxNum) break;
                _dicSemTokensModifiers.Add(eModifyVal.GetStr(), (int)eModifyVal);
            }

            // Hoverで表示するInstructionのSignatureを設定
            _instHashList = new InstructionHashList();
            _instSignature = new InstructionHint(signatureInputPath, _instHashList);
            _instSignature.MakeSignatureData();
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

        private void sendDiagnostics(string uri, List<Diagnostic_LSP> lstDiagnositcs)
        {
            if (_bIsPubDiagnostics) {
                var DiagnosticJson = new PublishDiagnosticsParams_LSP
                {
                    uri = uri,
                    diagnostics = new List<Diagnostic_LSP>(lstDiagnositcs)
                };

                sendMessage("textDocument/publishDiagnostics", DiagnosticJson);
            }

        }

        private void makeNewToken(string uri, string source)
        {
            Lexer lex = new Lexer(uri, source);
            lex.Tokenize(_instHashList);
            _dicLexer.Add(uri, lex);
        }

        private object commonInitialize(ClientCapabilities_LSP capabilities)
        {
            // ClientがPublishDiagnosticsに対応しているか
            if (capabilities.textDocument.publishDiagnostics != null) {
                _bIsPubDiagnostics = capabilities.textDocument.publishDiagnostics.relatedInformation;
            }

            // Clientが対応しているSemanticTokens
            if (capabilities.textDocument.semanticTokens != null) {
                foreach (string strVal in capabilities.textDocument.semanticTokens.tokenTypes) {
                    if (_dicSemTokensTypes.ContainsKey(strVal)) {
                        _bIsTokenTypes[_dicSemTokensTypes[strVal]] = true;
                    }
                }

                foreach (string strVal in capabilities.textDocument.semanticTokens.tokenModifiers) {
                    _bIsTokenModifiers[_dicSemTokensModifiers[strVal]] = true;
                }
            }
            // Serverが対応しているSemanticTokensの設定
            string[] astrTokenTypes = new string[(int)SemTokensTypeIdx.MaxNum];
            foreach (SemTokensTypeIdx eTypesVal in Enum.GetValues(typeof(SemTokensTypeIdx))) {
                if (eTypesVal == SemTokensTypeIdx.MaxNum) break;
                astrTokenTypes[(int)eTypesVal] = eTypesVal.GetStr();
            }

            string[] astrTokenModify = new string[(int)SemTokensModifyIdx.MaxNum];
            foreach (SemTokensModifyIdx eModifyVal in Enum.GetValues(typeof(SemTokensModifyIdx))) {
                if (eModifyVal == SemTokensModifyIdx.MaxNum) break;
                astrTokenModify[(int)eModifyVal] = eModifyVal.GetStr();
            }

            // Resultの作成
            var initResult = new ServerCapabilities_LSP
            {
                capabilities = new ServerCapabilities
                {
                    textDocumentSync = new TextDocumentSyncOptions
                    {
                        openClose = true,
                        change = TextDocumentSyncKind.Full
                    },
                    semanticTokensProvider = new SemanticTokensOptions
                    {
                        legend = new SemanticTokensOptions.SemanticTokensLegend
                        {
                            tokenTypes = astrTokenTypes,
                            tokenModifiers = astrTokenModify
                        },
                        range = false,
                        full = true
                    },
                    hoverProvider = _instSignature.IsSignatureDataEnable,
                }
            };
            sendLogMessage(MessageType.Info, $"Initialized Server ver:{_LSPServerVer}");
            return initResult;
        }

        // LSP Client for Visual Studio 2022用のInitialize
        public object initialize(int processId, string rootUri, ClientCapabilities_LSP capabilities)
        {
            return commonInitialize(capabilities);
        }

        // LSP Client for VS Code用のInitialize
        public object initialize(int processId, object clientInfo, string locale, string rootPath, string rootUri, ClientCapabilities_LSP capabilities, object trace, object workspaceFolders)
        {
            return commonInitialize(capabilities);
        }



        [JsonRpcMethod("textDocument/didOpen")]
        public void TextDocDidOpen(TextDocumentItem_LSP textDocument)
        {
            makeNewToken(textDocument.uri, textDocument.text);
        }

        [JsonRpcMethod("textDocument/didChange")]
        public void TextDocDidChange(VersionedTextDocumentIdentifier_LSP textDocument, TextDocumentContentChangeEvent_LSP[] contentChanges)
        {
            _dicLexer[textDocument.uri].InitToken(contentChanges[0].text);
            _dicLexer[textDocument.uri].Tokenize(_instHashList);
        }

        [JsonRpcMethod("textDocument/didClose")]
        public void TextDocDidClose(TextDocumentItem_LSP textDocument)
        {
            // 後始末
            _dicLexer[textDocument.uri].Dispose();
            _dicLexer.Remove(textDocument.uri);

            _lstDiagnostics.Clear();
            sendDiagnostics(textDocument.uri, _lstDiagnostics);

        }

        [JsonRpcMethod("textDocument/semanticTokens/full")]
        public object SemnticTokensFull(TextDocumentIdentifier_LSP textDocument)
        {
            var result = new SemanticTokens_LSP
            {
                data = new List<int>()
            };
            _dicLexer[textDocument.uri].MakeResult();
            _dicLexer[textDocument.uri].GetResultList(ref result.data);
            return result;
        }

        [JsonRpcMethod("textDocument/hover")]
        public object Hover(TextDocumentIdentifier_LSP textDocument, Position_LSP position)
        {
            string[] strContent = null;
            Token token = _dicLexer[textDocument.uri].SearchToken(position.line, position.character);

            if (token != null) {
                if (_instHashList.IsExistInstruction(token.TokenString)) {
                    strContent = new string[2] { null, null };
                    strContent[0] = _instSignature.GetSignature(token.TokenString);
                    strContent[1] = _instSignature.GetDescription(token.TokenString);
                } 
            }

            var result = new Hover_LSP
            {
                contents = strContent
            };

            return result;
        }


    }
}
    