using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace Shared
{
    public class TagKeyword
    {
        public const string LspNameSpace = "lsp-namespace";
        public const string LspClass = "lsp-class";
        public const string LspEnum = "lsp-enum";
        public const string LspInterface = "lsp-interface";
        public const string LspStruct = "lsp-struct";
        public const string LspTypeParameter = "lsp-typeparameter";
        public const string LspType = "lsp-type";
        public const string LspParameter = "lsp-parameter";
        public const string LspVariable = "lsp-variable";
        public const string LspProperty = "lsp-property";
        public const string LspEnumMember = "lsp-enummember";
        public const string LspDecorator = "lsp-decorator";
        public const string LspEvent = "lsp-event";
        public const string LspFunction = "lsp-function";
        public const string LspMethod = "lsp-method";
        public const string LspMacro = "lsp-macro";
        public const string LspLable = "lsp-label";
        public const string LspComment = "lsp-comment";
        public const string LspString = "lsp-string";
        public const string LspKeyword = "lsp-keyword";
        public const string LspNumber = "lsp-number";
        public const string LspRegexp = "lsp-regexp";
        public const string LspOperator = "lsp-operator";
        public const string LspModifier = "lsp-modifier";
        public const string LspInstruction = "lsp-instruction";

    }

    public enum SemTokensTypeIdx
    {
        [Classification(TagKeyword.LspNameSpace)]
        [StringVal("namespace")]
        NameSpace = 0,

        [Classification(TagKeyword.LspClass)]
        [StringVal("class")]
        Class = 1,

        [Classification(TagKeyword.LspEnum)]
        [StringVal("enum")]
        Enum = 2,

        [Classification(TagKeyword.LspInterface)]
        [StringVal("interface")]
        Interface = 3,

        [Classification(TagKeyword.LspStruct)]
        [StringVal("struct")]
        Struct = 4,

        [Classification(TagKeyword.LspTypeParameter)]
        [StringVal("typeParameter")]
        TypeParameter = 5,

        [Classification(TagKeyword.LspType)]
        [StringVal("type")]
        Type = 6,

        [Classification(TagKeyword.LspParameter)]
        [StringVal("parameter")]
        Parameter = 7,

        [Classification(TagKeyword.LspVariable)]
        [StringVal("variable")]
        Variable = 8,

        [Classification(TagKeyword.LspProperty)]
        [StringVal("property")]
        Property = 9,

        [Classification(TagKeyword.LspEnumMember)]
        [StringVal("enumMember")]
        EnumMember = 10,

        [Classification(TagKeyword.LspDecorator)]
        [StringVal("decorator")]
        Decorator = 11,

        [Classification(TagKeyword.LspEvent)]
        [StringVal("event")]
        Event = 12,

        [Classification(TagKeyword.LspFunction)]
        [StringVal("function")]
        Function = 13,

        [Classification(TagKeyword.LspMethod)]
        [StringVal("method")]
        Method = 14,

        [Classification(TagKeyword.LspMacro)]
        [StringVal("macro")]
        Macro = 15,

        [Classification(TagKeyword.LspLable)]
        [StringVal("label")]
        Lable = 16,

        [Classification(TagKeyword.LspComment)]
        [StringVal("comment")]
        Comment = 17,

        [Classification(TagKeyword.LspString)]
        [StringVal("string")]
        String = 18,

        [Classification(TagKeyword.LspKeyword)]
        [StringVal("keyword")]
        Keyword = 19,

        [Classification(TagKeyword.LspNumber)]
        [StringVal("number")]
        Number = 20,

        [Classification(TagKeyword.LspRegexp)]
        [StringVal("regexp")]
        Regexp = 21,

        [Classification(TagKeyword.LspOperator)]
        [StringVal("operator")]
        Operator = 22,

        [Classification(TagKeyword.LspModifier)]
        [StringVal("modifier")]
        Modifier = 23,

        [Classification(TagKeyword.LspInstruction)]
        [StringVal("instruction")] // オリジナルのTokenType
        Instruction = 24,

        MaxNum,
    }

    public enum SemTokensModifyIdx
    {
        [StringVal("declaration")]
        Declaration = 0,

        [StringVal("definition")]
        Definition = 1,

        [StringVal("readonly")]
        Readonly = 2,

        [StringVal("static")]
        Static = 3,

        [StringVal("deprecated")]
        Deprecated = 4,

        [StringVal("abstract")]
        Abstract = 5,

        [StringVal("async")]
        Async = 6,

        [StringVal("modification")]
        Modification = 7,

        [StringVal("documentation")]
        Documentation = 8,

        [StringVal("defaultLibrary")]
        DefaultLibrary = 9,

        MaxNum,
    }
}
