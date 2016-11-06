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
                new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start, span.Length)), this._classificationType)
            };

            return result;
        }

        #endregion
    }

    /// <summary>
    /// This class does everything that interacts with the VS editor.
    /// It inserts the class template code into the editor.
    /// </summary>
    static class ClassTemplateWriter //singleton for writing the class Templates into the editor
    {
        /// <summary>
        /// The abstract singleton private members. and set methods, templates
        /// </summary>
        private static class ClassTemplateWriterMembers 
        {
            public static void ResetSnapshotLength()
            {
                Codelength = View.TextBuffer.CurrentSnapshot.Length; //length of the current text on the editor. This is required for writing class templates in the bottom of the editor
            }

            public static IWpfTextView View;
            public static ITextEdit Edit;
            public static int Codelength;
            public static string ClassName;

                /// <summary>
                /// This method receives the current view and converts it to an IWpfTextView.
                /// </summary>
                /// <param name="txtManager"></param>
                /// <returns></returns>
            public static IWpfTextView GetWpfTextView(ref IVsTextManager txtManager)
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

            /// <summary>
            /// Templates for class declaration
            /// </summary>
            public static class ClassDeclarationTemplates
            {
                public static string classBasic()
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

                public static string classWithCopy()
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

                public static string classWithMove()
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
            public static class ClassDefinitionTemplates
            {
                public static string classBasic()
                {
                    return '\n' +
                           ClassName + "::" + ClassName + "() {}\n" +
                           ClassName + "::~" + ClassName + "() {}\n";
                }

                public static string classWithCopy()
                {
                    return '\n' +
                            ClassName + "::" + ClassName + "() {}\n" +
                            ClassName + "::" + ClassName + "(const " + ClassName +  "& other) {}\n" +
                            ClassName + "& "+ ClassName + "::" + "operator=(const " + ClassName +   "& other) {}\n" +
                            ClassName + "::~" + ClassName + "() {}\n";
                }

                public static string classWithMove()
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
            ClassTemplateWriterMembers.ClassName = className;
            ClassTemplateWriterMembers.View = ClassTemplateWriterMembers.GetWpfTextView(ref txtManager);
            ClassTemplateWriterMembers.Edit = ClassTemplateWriterMembers.View.TextBuffer.CreateEdit();
            ClassTemplateWriterMembers.ResetSnapshotLength();
        }

        public static class ClassDeclarationTemplates
        {
            public static void InsertClassBasic()
            {
                ClassTemplateWriterMembers.Edit.Insert(ClassTemplateWriterMembers.Codelength - 1,
                    ClassTemplateWriterMembers.ClassDeclarationTemplates.classBasic());
                ClassTemplateWriterMembers.Edit.Apply();
            }

            public static void InsertClassWithCopy()
            {
                ClassTemplateWriterMembers.Edit.Insert(ClassTemplateWriterMembers.Codelength - 1,
                    ClassTemplateWriterMembers.ClassDeclarationTemplates.classWithCopy());
                ClassTemplateWriterMembers.Edit.Apply();
            }

            public static void InsertClassWithMove()
            {
                ClassTemplateWriterMembers.Edit.Insert(ClassTemplateWriterMembers.Codelength - 1, //-1 is because of #endif. this might look like shit and it is shit. I'll change it later
                    ClassTemplateWriterMembers.ClassDeclarationTemplates.classWithMove());
                ClassTemplateWriterMembers.Edit.Apply();
            }
        }
        public static class ClassDefinitionTemplates
        {
            public static void InsertClassBasic()
            {
                ClassTemplateWriterMembers.Edit.Insert(ClassTemplateWriterMembers.Codelength,
                    ClassTemplateWriterMembers.ClassDefinitionTemplates.classBasic());
                ClassTemplateWriterMembers.Edit.Apply();
            }

            public static void InsertClassWithCopy()
            {
                ClassTemplateWriterMembers.Edit.Insert(ClassTemplateWriterMembers.Codelength,
                    ClassTemplateWriterMembers.ClassDefinitionTemplates.classWithCopy());
                ClassTemplateWriterMembers.Edit.Apply();
            }

            public static void InsertClassWithMove()
            {
                ClassTemplateWriterMembers.Edit.Insert(ClassTemplateWriterMembers.Codelength,
                    ClassTemplateWriterMembers.ClassDefinitionTemplates.classWithMove());
                ClassTemplateWriterMembers.Edit.Apply();
            }
        }
    }
}
