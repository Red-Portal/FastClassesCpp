//------------------------------------------------------------------------------
// <copyright file="EditorClassifier.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
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
        private static int Codelength;
        private static string ClassName;
        private static ClassTemplateWriter instance = null;

        private static ClassTemplateWriter getInstance()
        {
            return instance ?? (instance = new ClassTemplateWriter());
        }

        private ClassTemplateWriter()
        {
        }

        private static void ResetSnapshotLength()
        {
            Codelength = View.TextBuffer.CurrentSnapshot.Length;
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
                       "class " + ClassName + '\n' +
                       "{\n" +
                       "private:\n" +
                       "public:\n" +
                       ClassName + "();\n" +
                       '~' + ClassName + "();\n" +
                       "};";
            }

            public static string ClassWithCopy()
            {
                return '\n' +
                       "class " + ClassName + '\n' +
                       "{\n" +
                       "private:\n" +
                       "public:\n" +
                       ClassName + "();\n" +
                       ClassName + "(const " + ClassName + "& other);\n" +
                       ClassName + "& operator=(const " + ClassName + "& other);\n" +
                       '~' + ClassName + "();\n" +
                       "};";
            }

            public static string ClassWithMove()
            {
                return '\n' +
                       "class " + ClassName + '\n' +
                       "{\n" +
                       "private:\n" +
                       "public:\n" +
                       ClassName + "();\n" +
                       ClassName + "(const " + ClassName + "& other);\n" +
                       ClassName + "(" + ClassName + "&& other);\n" +
                       ClassName + "& operator=(const " + ClassName + "& other);\n" +
                       ClassName + "& operator=(" + ClassName + "&& other);\n" +
                       '~' + ClassName + "();\n" +
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
                       ClassName + "::" + ClassName + "() {}\n" +
                       ClassName + "::~" + ClassName + "() {}\n";
            }

            public static string ClassWithCopy()
            {
                return '\n' +
                       ClassName + "::" + ClassName + "() {}\n" +
                       ClassName + "::" + ClassName + "(const " + ClassName + "& other) {}\n" +
                       ClassName + "& " + ClassName + "::" + "operator=(const " + ClassName + "& other) {}\n" +
                       ClassName + "::~" + ClassName + "() {}\n";
            }

            public static string ClassWithMove()
            {
                return '\n' +
                       ClassName + "::" + ClassName + "() {}\n" +
                       ClassName + "::" + ClassName + "(const " + ClassName + "& other) {}\n" +
                       ClassName + "::" + ClassName + "(" + ClassName + "&& other) {}\n" +
                       ClassName + "& " + ClassName + "::" + "operator=(const " + ClassName + "& other) {}\n" +
                       ClassName + "& " + ClassName + "::" + "operator=(" + ClassName + "&& other) {}\n" +
                       ClassName + "::~" + ClassName + "() {}\n";
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
            ClassName = className;
            View = GetWpfTextView(ref txtManager);
            Edit = View.TextBuffer.CreateEdit();
            ResetSnapshotLength();
        }

        /// <summary>
        /// static class containing the implementation for the public methods
        /// The implementation is disconnected like this to relieve the user from using 'getInstance'
        /// </summary>
        private static class Implementation
        {
            public static class Declaration
            {
                public static void InsertClassBasic()
                {
                    Edit.Insert(Codelength - 1, DeclarationTemplate.ClassBasic());
                    Edit.Apply();
                }

                public static void InsertClassWithCopy()
                {
                    Edit.Insert(Codelength - 1, DeclarationTemplate.ClassWithCopy());
                    Edit.Apply();
                }

                public static void InsertClassWithMove()
                {
                    Edit.Insert(Codelength - 1, DeclarationTemplate.ClassWithMove());
                    Edit.Apply();
                    //-1 is because of #endif. this might look like shit and it is shit. I'll change it later
                }
            }

            public static class Definition
            {
                public static void InsertClassBasic()
                {
                    Edit.Insert(Codelength, DefinitionTemplate.ClassBasic());
                    Edit.Apply();
                }

                public static void InsertClassWithCopy()
                {
                    Edit.Insert(Codelength, DefinitionTemplate.ClassWithCopy());
                    Edit.Apply();
                }

                public static void InsertClassWithMove()
                {
                    Edit.Insert(Codelength, DefinitionTemplate.ClassWithMove());
                    Edit.Apply();
                }
            }
        }

        public static class ClassDeclarationTemplates
        {
            public static void InsertClassBasic()
            {
               Implementation.Declaration.InsertClassBasic();
            }

            public static void InsertClassWithCopy()
            {
                Implementation.Declaration.InsertClassWithCopy();
            }

            public static void InsertClassWithMove()
            {
                Implementation.Declaration.InsertClassWithMove();
            }
        }

        public static class ClassDefinitionTemplates
        {
            public static void InsertClassBasic()
            {
               Implementation.Definition.InsertClassBasic();
            }

            public static void InsertClassWithCopy()
            {
                Implementation.Definition.InsertClassWithCopy();
            }

            public static void InsertClassWithMove()
            {
                Implementation.Definition.InsertClassWithMove();
            }
        }
    }
}