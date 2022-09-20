using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Documents;
using Shared;
using System.Diagnostics;

namespace LspClientForVS2022
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("ore")]
    [TagType(typeof(ClassificationTag))]
    public sealed class BeginTemplateTaggerProvider  : IViewTaggerProvider
    {
        [Import]
        public IClassificationTypeRegistryService Registry;

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            // .oreファイルを開いたときにまず呼ばれる。この時まだエディタには何も表示されていない状態
            if (buffer != textView.TextBuffer) {
                return null;
            }

            var classTypeList = new List<IClassificationType>();
            foreach (SemTokensTypeIdx eTypes in Enum.GetValues(typeof(SemTokensTypeIdx))) {
                if (eTypes == SemTokensTypeIdx.MaxNum) break;
                classTypeList.Add(Registry.GetClassificationType(eTypes.GetClassification()));
            }

            string uri = textView.TextBuffer.GetUri();
            return new LspTagger(textView, uri, classTypeList) as ITagger<T>;
        }
    }

}
