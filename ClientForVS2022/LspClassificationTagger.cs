using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;
using Shared;
using System.Windows.Documents;
using System.Diagnostics;

namespace ClientForVS2022
{
    public class SemTokenDecodeData
    {
        public readonly IClassificationType m_ClassTagType;
        public readonly SnapshotSpan m_SnapshotSpan;

        public SemTokenDecodeData(IClassificationType tagType, SnapshotSpan span)
        {
            m_ClassTagType = tagType;
            m_SnapshotSpan = span;
        }
    }
    public sealed class LspTagger : ITagger<ClassificationTag>
    {
        private readonly ITextView m_View;
        private readonly List<IClassificationType> m_ClassTagTypeList = new List<IClassificationType>();
        private readonly string m_uri;
        private ITextSnapshot m_snapshot;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged = delegate { };

        public LspTagger(ITextView view, string uri, List<IClassificationType> tagTypeList)
        {
            m_View = view;
            m_View.LayoutChanged += ViewLayoutChanged;
            m_ClassTagTypeList = tagTypeList;
            m_uri = uri;
            m_snapshot = view.TextSnapshot;
        }

        private void ViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (e.OldSnapshot != e.NewSnapshot) {
                m_snapshot = e.NewSnapshot;
            }
        }


        private IEnumerable<SemTokenDecodeData> DecodeSemTokenData(int[] semTokensData)
        {
            int iCumSum_Length = 0;
            int iCumSum_Row = 0;
            int iLineLength = 0;

            for (int idx = 0; idx < semTokensData.Length;) {
                int iDeltaLine = semTokensData[idx + 0];
                int iDeltaStart = semTokensData[idx + 1];
                int iLength = semTokensData[idx + 2];
                int iTokenType = semTokensData[idx + 3];
                int iTokenModifiers = semTokensData[idx + 4];
                idx += 5;

                if (iDeltaLine > 0) {
                    for (int i = 0; i < iDeltaLine; i++) {
                        iCumSum_Length += m_snapshot.GetLineFromLineNumber(iCumSum_Row + i).LengthIncludingLineBreak;
                    }
                    iCumSum_Row += iDeltaLine;
                    iLineLength = iDeltaStart;
                } else {
                    iLineLength += iDeltaStart;
                }

                int iSpanStart = iCumSum_Length + iLineLength;

                if (((iSpanStart + iLength) <= m_snapshot.Length) && (iTokenType < m_ClassTagTypeList.Count) && (m_ClassTagTypeList[iTokenType] != null)) {
                    SnapshotSpan span = new SnapshotSpan(m_snapshot, iSpanStart, iLength);
                    yield return new SemTokenDecodeData(m_ClassTagTypeList[iTokenType], span);
                }

            }
        }

        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans == null || spans.Count == 0) {
                yield break;
            }

            int[] semTokensData;
            MyLanguageClient.GetSemTokensData(m_uri, out semTokensData);
            if (semTokensData == null) {
                yield break;
            }
            foreach (var temp in DecodeSemTokenData(semTokensData)) {
                yield return new TagSpan<ClassificationTag>(temp.m_SnapshotSpan, new ClassificationTag(temp.m_ClassTagType));
            }
        }
    }
}
