﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace LspClientForVS2022
{
    public class MyContentDefinition
    {
        [Export]
        [Name("ore")]
        [BaseDefinition(CodeRemoteContentDefinition.CodeRemoteContentTypeName)]
        internal static ContentTypeDefinition MyContentTypeDefinition;

        [Export]
        [FileExtension(".ore")]
        [ContentType("ore")]
        internal static FileExtensionToContentTypeDefinition MyFileExtensionDefinition;
    }
}