//------------------------------------------------------------------------------
// <copyright file="EditorClassifier.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

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
        private readonly IClassificationType _classificationType;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorClassifier"/> class.
        /// </summary>
        /// <param name="registry">Classification registry.</param>
        internal EditorClassifier(IClassificationTypeRegistryService registry)
        {
            this._classificationType = registry.GetClassificationType("EditorClassifier");
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
                new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start, span.Length)),
                    this._classificationType)
            };

            return result;
        }

        #endregion
    }

    public class BaseTemplateWriter
    {
        protected static IWpfTextView View;
        protected static ITextEdit Edit;
        protected static IWpfTextView GetWpfTextView(ref IVsTextManager txtManager)
        {
            IVsTextView currentView;
            txtManager.GetActiveView(1, null, out currentView);

            IWpfTextView view = null;
            IVsUserData userData = currentView as IVsUserData;

            if (null != userData)
            {
                IWpfTextViewHost viewHost;
                object holder;
                Guid guidViewHost = DefGuidList.guidIWpfTextViewHost;
                userData.GetData(ref guidViewHost, out holder);
                viewHost = (IWpfTextViewHost) holder;
                view = viewHost.TextView;
            }
            return view;
        }
    }

    /// <summary>
    /// This singleton does everything that interacts with the VS editor.
    /// It inserts the class template code into the editor.
    /// </summary>
    public class ClassTemplateWriter : BaseTemplateWriter
    {
        private static int m_codelength;
        private static string m_className;
        private static ClassTemplateWriter instance = null;

        private static void ResetSnapshotLength()
        {
            m_codelength = View.TextBuffer.CurrentSnapshot.Length;
            //length of the current text on the editor. This is required for writing class templates in the bottom of the editor
        }

        /// <summary>
        /// Templates for class declaration
        /// </summary>
        private static class DeclarationTemplate
        {
            public static string ClassBasic()
            {
                return '\n' +
                       "class " + m_className + '\n' +
                       "{\n" +
                       "private:\n" +
                       "public:\n" +
                       m_className + "();\n" +
                       '~' + m_className + "();\n" +
                       "};";
            }

            public static string ClassWithCopy()
            {
                return '\n' +
                       "class " + m_className + '\n' +
                       "{\n" +
                       "private:\n" +
                       "public:\n" +
                       m_className + "();\n" +
                       m_className + "(const " + m_className + "& other);\n" +
                       m_className + "& operator=(const " + m_className + "& other);\n" +
                       '~' + m_className + "();\n" +
                       "};";
            }

            public static string ClassWithMove()
            {
                return '\n' +
                       "class " + m_className + '\n' +
                       "{\n" +
                       "private:\n" +
                       "public:\n" +
                       m_className + "();\n" +
                       m_className + "(const " + m_className + "& other);\n" +
                       m_className + "(" + m_className + "&& other);\n" +
                       m_className + "& operator=(const " + m_className + "& other);\n" +
                       m_className + "& operator=(" + m_className + "&& other);\n" +
                       '~' + m_className + "();\n" +
                       "};";
            }
        }

        /// <summary>
        /// Templates for class definition
        /// </summary>
        private static class DefinitionTemplate
        {
            public static string ClassBasic()
            {
                return '\n' +
                       m_className + "::" + m_className + "() {}\n" +
                       m_className + "::~" + m_className + "() {}\n";
            }

            public static string ClassWithCopy()
            {
                return '\n' +
                       m_className + "::" + m_className + "() {}\n" +
                       m_className + "::" + m_className + "(const " + m_className + "& other) {}\n" +
                       m_className + "& " + m_className + "::" + "operator=(const " + m_className + "& other) {}\n" +
                       m_className + "::~" + m_className + "() {}\n";
            }

            public static string ClassWithMove()
            {
                return '\n' +
                       m_className + "::" + m_className + "() {}\n" +
                       m_className + "::" + m_className + "(const " + m_className + "& other) {}\n" +
                       m_className + "::" + m_className + "(" + m_className + "&& other) {}\n" +
                       m_className + "& " + m_className + "::" + "operator=(const " + m_className + "& other) {}\n" +
                       m_className + "& " + m_className + "::" + "operator=(" + m_className + "&& other) {}\n" +
                       m_className + "::~" + m_className + "() {}\n";
            }
        }

        /// <summary>
        /// initialize the hidden members of the singleton. This class MUST BE CALLED EVERY TIME
        /// THERE IS A CHANGE IN THE DATA OF THE MODAL DIALOG BOX.
        /// Get the input ClassName and initialize all the templates,
        /// reset the editor text snapshot length so we properly find the new 'end' of the text.
        /// </summary>
        /// <param name="className"></param>
        public static void initializeMembers(string className, ref IVsTextManager txtManager)
        {
            if(instance == null)
                instance = new ClassTemplateWriter();
            m_className = className;
            View = GetWpfTextView(ref txtManager);
            Edit = View.TextBuffer.CreateEdit();
            ResetSnapshotLength();
        }

        /// <summary>
        /// Receive the ID of the called command and documen type, handle the command event accordingly 
        /// </summary>
        /// <param name="cmdGuid"></param>
        /// <param name="documentType"></param>
        public static void CommandHandler(int cmdGuid, int documentType)
        {
            switch (cmdGuid)
            {
                default:
                    break;
                case CmdGuid.CommandMakeClassBasic:
                {
                    if (documentType == 1)
                        Edit.Insert(m_codelength, DefinitionTemplate.ClassBasic());
                    else if (documentType == 2)
                        Edit.Insert(m_codelength, DeclarationTemplate.ClassBasic());
                    break;
                }
                case CmdGuid.CommandMakeClassWithCopy:
                {
                    if (documentType == 1)
                        Edit.Insert(m_codelength, DefinitionTemplate.ClassWithCopy());
                    else if (documentType == 2)
                        Edit.Insert(m_codelength, DeclarationTemplate.ClassWithCopy());
                    break;
                }
                case CmdGuid.CommandMakeClassWithMove:
                {
                    if (documentType == 1)
                        Edit.Insert(m_codelength, DefinitionTemplate.ClassWithMove());
                    else if (documentType == 2)
                        Edit.Insert(m_codelength, DeclarationTemplate.ClassWithMove());
                    break;
                }
            }
            Edit.Apply();
        }
    }

    public class MemberVarTemplateWriter : BaseTemplateWriter
    {
        private static MemberVarTemplateWriter instance = null;

        public void initializeMembers(ref IVsTextManager txtManager)
        {
            if(instance == null)
                instance = new MemberVarTemplateWriter();
            View = GetWpfTextView(ref txtManager);
            Edit = View.TextBuffer.CreateEdit();
        }
    }
}