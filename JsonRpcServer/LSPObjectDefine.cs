using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonRpcServer
{
    /* --- LSP仕様書で定義されているJSONフォーマット定義 --- */

    /* --- データ型 --- */
    public class TextDocumentItem_LSP
    {
        public string uri;
        public string languageId;
        public int version;
        public string text;
    }
    public sealed class Position_LSP
    {
        public int line;
        public int character;
    }
    public sealed class Range
    {
        public Range(int startLine, int startChar, int endLine, int endChar)
        {
            this.start.line = startLine;
            this.start.character = startChar;

            this.end.line = endLine;
            this.end.character = endChar;
        }
        
        public Position_LSP start;
        public Position_LSP end;
}

    public sealed class Diagnostic_LSP
    {
        public Range range;
        public string message;
        public DiagnosticSeverity severity;
    }

    public sealed class PublishDiagnosticsParams_LSP
    {
        public string uri;
        public List<Diagnostic_LSP> diagnostics;
    }

 

    public sealed class WorkspaceClientCapabilities
    {
        public bool applyEdit;

        public struct WorkspaceEditCapabilities { public bool documentChanges; }
        public WorkspaceEditCapabilities? documentChanges;

        public struct DidConfigurationChangeCapabilities { public bool dynamicRegistration; }
        public DidConfigurationChangeCapabilities? didConfigurationChange;

        public struct DidChangeWatchedFilesCapabilities { public bool dynamicRegistration; }
        public DidChangeWatchedFilesCapabilities? didChangeWatchedFiles;

        public struct SymbolCapabilities
        {
            public bool dynamicRegistration;

            public struct SymbolKindCapabilities
            {
                /// <summary>
                /// The symbol kind values the client supports. When this
                /// property exists the client also guarantees that it will
                /// handle values outside its set gracefully and falls back
                /// to a default value when unknown.
                /// 
                /// If this property is not present the client only supports
                /// the symbol kinds from `File` to `Array` as defined in
                /// the initial version of the protocol.
                /// </summary>
                public SymbolKind[] valueSet;
            }
            public SymbolKindCapabilities? symbolKind;
        }

        public SymbolCapabilities? symbol;

        public struct ExecuteCommandCapabilities { public bool dynamicRegistration; }
        public ExecuteCommandCapabilities? executeCommand;

        public bool? configuration;
    }

    public sealed class TextDocumentClientCapabilities
    {
        public struct SynchronizationCapabilities
        {
            public bool dynamicRegistration;
            public bool willSave;
            /// <summary>
            /// The client supports sending a will save request and
            /// waits for a response providing text edits which will
            /// be applied to the document before it is saved.
            /// </summary>
            public bool willSaveWaitUntil;
            public bool didSave;
        }
        public SynchronizationCapabilities? synchronization;

        public sealed class CompletionCapabilities
        {
            public bool dynamicRegistration;

            public sealed class CompletionItemCapabilities
            {
                /// <summary>
                /// Client supports snippets as insert text.
                /// 
                /// A snippet can define tab stops and placeholders with `$1`, `$2`
                /// and `${3:foo}`. `$0` defines the final tab stop, it defaults to
                /// the end of the snippet. Placeholders with equal identifiers are linked,
                /// that is typing in one will update others too.
                /// </summary>
                public bool snippetSupport;

                public bool commitCharactersSupport;

                public string[] documentationFormat;
            }
            public CompletionItemCapabilities completionItem;

            public sealed class CompletionItemKindCapabilities
            {
                /// <summary>
                /// The completion item kind values the client supports. When this
                /// property exists the client also guarantees that it will
                /// handle values outside its set gracefully and falls back
                /// to a default value when unknown.
                /// 
                /// If this property is not present the client only supports
                /// the completion items kinds from `Text` to `Reference` as defined in
                /// the initial version of the protocol.
                /// </summary>
                public SymbolKind[] valueSet;
            }
            public CompletionItemKindCapabilities completionItemKind;

            /// <summary>
            /// The client supports to send additional context information for a
            /// `textDocument/completion` request.
            /// </summary>
            public bool contextSupport;
        }
        public CompletionCapabilities completion;

        public sealed class HoverCapabilities
        {
            public bool dynamicRegistration;
            /// <summary>
            /// Client supports the follow content formats for the content
            /// property.The order describes the preferred format of the client.
            /// </summary>
            public string[] contentFormat;
        }
        public HoverCapabilities hover;

        public sealed class SignatureHelpCapabilities
        {
            public bool dynamicRegistration;

            public struct SignatureInformationCapabilities
            {
                /// <summary>
                ///  Client supports the follow content formats for the documentation
                /// property.The order describes the preferred format of the client.
                /// </summary>
                public string[] documentationFormat;

                /// <summary>
                /// Client capabilities specific to parameter information.
                /// </summary>
                public struct ParameterInformationCapabilities
                {
                    /// <summary>
                    ///  The client supports processing label offsets instead of a simple label string
                    /// </summary>
                    public bool? labelOffsetSupport;
                }
                public ParameterInformationCapabilities? parameterInformation;
            }
            public SignatureInformationCapabilities? signatureInformation;
        }
        public SignatureHelpCapabilities signatureHelp;

        public sealed class ReferencesCapabilities { public bool dynamicRegistration; }
        public ReferencesCapabilities references;

        public sealed class DocumentHighlightCapabilities { public bool dynamicRegistration; }
        public DocumentHighlightCapabilities documentHighlight;

        public sealed class DocumentSymbolCapabilities
        {
            public bool dynamicRegistration;
            public sealed class SymbolKindCapabilities
            {
                /// <summary>
                /// The symbol kind values the client supports. When this
                /// property exists the client also guarantees that it will
                /// handle values outside its set gracefully and falls back
                /// to a default value when unknown.
                /// 
                /// If this property is not present the client only supports
                /// the symbol kinds from `File` to `Array` as defined in
                /// the initial version of the protocol.
                /// </summary>
                public SymbolKind[] valueSet;
            }
            public SymbolKindCapabilities symbolKind;

            /// <summary>
            /// The client support hierarchical document symbols.
            /// </summary>
            public bool? hierarchicalDocumentSymbolSupport;
        }
        public DocumentSymbolCapabilities documentSymbol;

        public sealed class FormattingCapabilities { public bool dynamicRegistration; }
        public FormattingCapabilities formatting;

        public sealed class RangeFormattingCapabilities { public bool dynamicRegistration; }
        public RangeFormattingCapabilities rangeFormatting;

        public sealed class OnTypeFormattingCapabilities { public bool dynamicRegistration; }
        public OnTypeFormattingCapabilities onTypeFormatting;

        public sealed class DefinitionCapabilities { public bool dynamicRegistration; }
        public DefinitionCapabilities definition;

        public sealed class CodeActionCapabilities
        {
            public bool dynamicRegistration;
            //    
            // The client support code action literals as a valid
            // response of the `textDocument/codeAction` request.
            // 
            // Since 3.8.0
            // 
            public class CodeActionLiteralSupport
            {
                // 
                // The code action kind is support with the following value
                // set.
                // 
                public class CodeActionKind
                {
                    // 
                    // The code action kind values the client supports. When this
                    // property exists the client also guarantees that it will
                    // handle values outside its set gracefully and falls back
                    // to a default value when unknown.
                    // 
                    public string[] valueSet;
                }
                public CodeActionKind codeActionKind;
            }
            public CodeActionLiteralSupport codeActionLiteralSupport;
        }
        public CodeActionCapabilities codeAction;

        public sealed class CodeLensCapabilities { public bool dynamicRegistration; }
        public CodeLensCapabilities codeLens;

        public sealed class DocumentLinkCapabilities { public bool dynamicRegistration; }
        public DocumentLinkCapabilities documentLink;

        public sealed class RenameCapabilities { public bool dynamicRegistration; }
        public RenameCapabilities rename;

        public sealed class PublishDiagnosticsClientCapabilities { public bool relatedInformation; }
        public PublishDiagnosticsClientCapabilities publishDiagnostics;

        public sealed class SemanticTokensClientCapabilities
        {
            public string[] tokenTypes;
            public string[] tokenModifiers;

        }
        public SemanticTokensClientCapabilities semanticTokens;


    }

    public sealed class ClientCapabilities_LSP
    {
        public WorkspaceClientCapabilities workspace;
        public TextDocumentClientCapabilities textDocument;

    }

    public sealed class TextDocumentSyncOptions
    {
        public bool openClose;
        public TextDocumentSyncKind change;
    }

    public sealed class SemanticTokensOptions
    {
        public sealed class SemanticTokensLegend
        {
            public string[] tokenTypes;
            public string[] tokenModifiers;
        }
        public SemanticTokensLegend legend;
        public bool range;
        public bool full;
    }

    public sealed class ServerCapabilities
    {
        public TextDocumentSyncOptions textDocumentSync;
        public SemanticTokensOptions semanticTokensProvider;
        public bool hoverProvider;
    }

    public sealed class ServerCapabilities_LSP
    {
        public ServerCapabilities capabilities;
    }

    public sealed class LogMessageParams_LSP
    {
        public MessageType type;
        public string message;
    }

    public sealed class TextDocumentIdentifier_LSP
    {
        public string uri;
    }

    public sealed class SemanticTokens_LSP
    {
        public List<int> data;
    }

    public sealed class Hover_LSP
    {
        public string[] contents;
    }

    /* --- enum --- */
    public enum DiagnosticSeverity
    {
        Error = 1,
        Warning = 2,
        Information = 3,
        Hint = 4,
    }

    public enum SymbolKind
    {
        None = 0,
        File = 1,
        Module = 2,
        Namespace = 3,
        Package = 4,
        Class = 5,
        Method = 6,
        Property = 7,
        Field = 8,
        Constructor = 9,
        Enum = 10,
        Interface = 11,
        Function = 12,
        Variable = 13,
        Constant = 14,
        String = 15,
        Number = 16,
        Boolean = 17,
        Array = 18,
        Object = 19,
        Key = 20,
        Null = 21,
        EnumMember = 22,
        Struct = 23,
        Event = 24,
        Operator = 25,
        TypeParameter = 26
    }

    public enum TextDocumentSyncKind
    {
        None = 0,
        Full = 1,
        Incremental = 2
    }

    public enum MessageType
    {
        Error = 1,
        Warning = 2,
        Info = 3,
        Log = 4
    }
}
