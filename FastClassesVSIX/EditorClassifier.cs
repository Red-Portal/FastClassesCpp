//------------------------------------------------------------------------------
// <copyright file="EditorClassifier.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace FastClassesVSIX
{
    /// <summary>
    /// Classifier that classifies all text as an instance of the "EditorClassifier" classification type.
    /// </summary>
    internal class EditorClassifier : IClassifier
    {
        /// <summary>
        /// Classification type.
        /// </summary>
        private readonly IClassificationType classificationType;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorClassifier"/> class.
        /// </summary>
        /// <param name="registry">Classification registry.</param>
        internal EditorClassifier(IClassificationTypeRegistryService registry)
        {
            this.classificationType = registry.GetClassificationType("EditorClassifier");
        }

        #region IClassifier

#pragma warning disable 67

        /// <summary>
        /// An event that occurs when the classification of a span of text has changed.
        /// </summary>
        /// <remarks>
        /// This event gets raised if a non-text change would affect the classification in some way,
        /// for example typing /* would cause the classification to change in C# without directly
        /// affecting the span.
        /// </remarks>
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

#pragma warning restore 67

        /// <summary>
        /// Gets all the <see cref="ClassificationSpan"/> objects that intersect with the given range of text.
        /// </summary>
        /// <remarks>
        /// This method scans the given SnapshotSpan for potential matches for this classification.
        /// In this instance, it classifies everything and returns each span as a new ClassificationSpan.
        /// </remarks>
        /// <param name="span">The span currently being classified.</param>
        /// <returns>A list of ClassificationSpans that represent spans identified to be of this classification.</returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var result = new List<ClassificationSpan>()
            {
                new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start, span.Length)), this.classificationType)
            };

            return result;
        }

        #endregion
    }

    [ContentType("Code")]
    [Export(typeof(IWpfTextViewCreationListener))]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal sealed class TextViewCreationListener : IWpfTextViewCreationListener
    {
        public void TextViewCreated(IWpfTextView textView)
        {
            ClassFormWriter.initializeSingletonFromParent(textView);
        }
    }

    public sealed class ClassFormWriter
    {
        private IWpfTextView m_view;
        private static ClassFormWriter classFormWriterInstance;
        static public void initializeSingletonFromParent(IWpfTextView view)
        {
            classFormWriterInstance = new ClassFormWriter(view);
        }
        private ClassFormWriter(IWpfTextView view)
        {
            m_view = view;
        }
        static public ClassFormWriter getClassFormWriter()
        {
            return classFormWriterInstance;
        }

        public void fastClassOption1()
        {
            ITextEdit edit = m_view.TextBuffer.CreateEdit();
            edit.Insert(0, "class ClassName\n" +
                           "{\n" +
                           "ClassName() = default;\n" +
                           "~ClassName() = default;\n" +
                           "}\n");
            edit.Apply();
        }
    }
    /*
    class Formatter
    {
        private IWpfTextView _view;
        private bool _isChangingText = false;
        public Formatter(IWpfTextView view)
        {
            _view = view;
            _view.TextBuffer.Changed += new EventHandler<TextContentChangedEventArgs>(TextBuffer_Changed);
            _view.TextBuffer.PostChanged += new EventHandler(TextBuffer_PostChanged);
        }

        private void TextBuffer_Changed(object sender, TextContentChangedEventArgs e)
        {
            if (!_isChangingText)
            {
                _isChangingText = true;
                FormatCode(e);
            }
        }

        private void TextBuffer_PostChanged(object sender, EventArgs e)
        {
            if (_isChangingText)
                _isChangingText = false;
        }

        private void FormatCode(TextContentChangedEventArgs e)
        {
            if (e.Changes != null)
            {
                for (int i = 0; i < e.Changes.Count; i++)
                {
                    HandleChange(e.Changes[0].NewText);
                }
            }

        }

        private void HandleChange(string newText)
        {
            ITextEdit edit = _view.TextBuffer.CreateEdit();
            edit.Insert(0, "Hello");
            edit.Apply();
        }
        */

}