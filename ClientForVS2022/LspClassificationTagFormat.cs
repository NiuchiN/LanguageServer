using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Shared;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace ClientForVS2022
{
    public static class TypeExports
    {

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(TagKeyword.LspVariable)]
        public static ClassificationTypeDefinition LspVariableDef;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(TagKeyword.LspFunction)]
        public static ClassificationTypeDefinition LspFunctionDef;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(TagKeyword.LspMethod)]
        public static ClassificationTypeDefinition LspMethodDef;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(TagKeyword.LspMacro)]
        public static ClassificationTypeDefinition LspMacroDef;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(TagKeyword.LspLable)]
        public static ClassificationTypeDefinition LspLableDef;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(TagKeyword.LspComment)]
        public static ClassificationTypeDefinition LspCommentDef;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(TagKeyword.LspString)]
        public static ClassificationTypeDefinition LspStringDef;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(TagKeyword.LspKeyword)]
        public static ClassificationTypeDefinition LspKeywordDef;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(TagKeyword.LspNumber)]
        public static ClassificationTypeDefinition LspNumberDef;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(TagKeyword.LspInstruction)]
        public static ClassificationTypeDefinition LspInstructionDef;

    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TagKeyword.LspVariable)]
    [Name(TagKeyword.LspVariable)]
    [UserVisible(true)] // This should be visible to the end user
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    internal sealed class LspVariableFormat : ClassificationFormatDefinition
    {
        public LspVariableFormat()
        {
            this.DisplayName = TagKeyword.LspVariable;
            this.ForegroundColor = Colors.DarkTurquoise;
            //this.BackgroundColor = Colors.BlueViolet;
            //this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TagKeyword.LspFunction)]
    [Name(TagKeyword.LspFunction)]
    [UserVisible(true)] // This should be visible to the end user
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    internal sealed class LspFunctionFormat : ClassificationFormatDefinition
    {
        public LspFunctionFormat()
        {
            this.DisplayName = TagKeyword.LspFunction;
            this.ForegroundColor = Colors.Gold;
            //this.BackgroundColor = Colors.BlueViolet;
            //this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TagKeyword.LspMethod)]
    [Name(TagKeyword.LspMethod)]
    [UserVisible(true)] // This should be visible to the end user
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    internal sealed class LspMethodFormat : ClassificationFormatDefinition
    {
        public LspMethodFormat()
        {
            this.DisplayName = TagKeyword.LspMethod;
            this.ForegroundColor = Colors.Gold;
            //this.BackgroundColor = Colors.BlueViolet;
            //this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TagKeyword.LspMacro)]
    [Name(TagKeyword.LspMacro)]
    [UserVisible(true)] // This should be visible to the end user
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    internal sealed class LspMacroFormat : ClassificationFormatDefinition
    {
        public LspMacroFormat()
        {
            this.DisplayName = TagKeyword.LspMacro;
            this.ForegroundColor = Colors.Gold;
            //this.BackgroundColor = Colors.BlueViolet;
            //this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TagKeyword.LspLable)]
    [Name(TagKeyword.LspLable)]
    [UserVisible(true)] // This should be visible to the end user
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    internal sealed class LspLableFormat : ClassificationFormatDefinition
    {
        public LspLableFormat()
        {
            this.DisplayName = TagKeyword.LspLable;
            this.ForegroundColor = Colors.Coral;
            //this.BackgroundColor = Colors.BlueViolet;
            //this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TagKeyword.LspComment)]
    [Name(TagKeyword.LspComment)]
    [UserVisible(true)] // This should be visible to the end user
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    internal sealed class LspCommentFormat : ClassificationFormatDefinition
    {
        public LspCommentFormat()
        {
            this.DisplayName = TagKeyword.LspComment;
            this.ForegroundColor = Colors.LimeGreen;
            //this.BackgroundColor = Colors.BlueViolet;
            //this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TagKeyword.LspString)]
    [Name(TagKeyword.LspString)]
    [UserVisible(true)] // This should be visible to the end user
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    internal sealed class LspStringFormat : ClassificationFormatDefinition
    {
        public LspStringFormat()
        {
            this.DisplayName = TagKeyword.LspString;
            this.ForegroundColor = Colors.Salmon;
            //this.BackgroundColor = Colors.BlueViolet;
            //this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TagKeyword.LspKeyword)]
    [Name(TagKeyword.LspKeyword)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class LspKeywordFormat : ClassificationFormatDefinition
    {
        public LspKeywordFormat()
        {
            this.DisplayName = TagKeyword.LspKeyword;
            this.ForegroundColor = Colors.Violet;
            //this.BackgroundColor = Colors.BlueViolet;
            //this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TagKeyword.LspNumber)]
    [Name(TagKeyword.LspNumber)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class LspNumberFormat : ClassificationFormatDefinition
    {
        public LspNumberFormat()
        {
            this.DisplayName = TagKeyword.LspNumber;
            this.ForegroundColor = Colors.Honeydew;
            //this.BackgroundColor = Colors.BlueViolet;
            //this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }


    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TagKeyword.LspInstruction)]
    [Name(TagKeyword.LspInstruction)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class LspInstructionFormat : ClassificationFormatDefinition
    {
        public LspInstructionFormat()
        {
            this.DisplayName = TagKeyword.LspInstruction;
            this.ForegroundColor = Colors.Honeydew;
            //this.BackgroundColor = Colors.BlueViolet;
            //this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }

}
