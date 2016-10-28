//------------------------------------------------------------------------------
// <copyright file="FastClassesOptionWindow.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace FastClassesVSIX
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("0f7092cf-7a07-4a69-a5a6-e879bfe1ed3f")]
    public class FastClassesOptionWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FastClassesOptionWindow"/> class.
        /// </summary>
        public FastClassesOptionWindow() : base(null)
        {
            this.Caption = "FastClassesOptionWindow";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new FastClassesOptionWindowControl();
        }
    }
}
