using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LspClientForVS2022
{
    public static class Helper
    {
        public static string GetUri(this ITextBuffer buffer)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (buffer == null) {
                return null;
            }

            Microsoft.VisualStudio.Text.Projection.IElisionBuffer projection = buffer as Microsoft.VisualStudio.Text.Projection.IElisionBuffer;
            if (projection != null) {
                ITextBuffer source_buffer = projection.SourceBuffer;
                return source_buffer.GetUri();
            }
            buffer.Properties.TryGetProperty(typeof(Microsoft.VisualStudio.TextManager.Interop.IVsTextBuffer), out Microsoft.VisualStudio.TextManager.Interop.IVsTextBuffer bufferAdapter);
            if (bufferAdapter != null) {
                Microsoft.VisualStudio.Shell.Interop.IPersistFileFormat persistFileFormat = bufferAdapter as Microsoft.VisualStudio.Shell.Interop.IPersistFileFormat;
                string ppzsFilename = null;
                if (persistFileFormat != null) {
                    persistFileFormat.GetCurFile(out ppzsFilename, out uint iii);
                }

                ppzsFilename = @"file:///" + ppzsFilename.Replace(@"\", @"/");

                return ppzsFilename;
            }
            return null;
        }
    
    }
}
