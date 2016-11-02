//------------------------------------------------------------------------------
// <copyright file="EditorClassifier.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
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
            ClassTemplateWriter.initializeFromParent(textView);
        }
    }

    static class ClassTemplateWriter //singleton for writing the class Templates into the editor
    {
        private static class ClassTemplateWriterMembers //the abstracted singleton members etc..
        {
            
            public static void resetSnapshotLength()
            {
                Codelength = view.TextBuffer.CurrentSnapshot.Length; //length of the current text on the editor. This is required for writing class templates in the bottom of the editor
                edit = view.TextBuffer.CreateEdit();
            }

            public static IWpfTextView view;
            public static ITextEdit edit; 
            public static int Codelength;
            public static string className;

            public static class ClassDeclarationTemplates
            {
                public static string classBasic()
                {
                    return '\n' +
                           "class " + className + '\n' +
                           "{\n" +
                           "private:\n" +
                           "public:\n" +
                           className + "();\n" +
                           '~' + className + "();\n" +
                           '}';
                }

                public static string classWithCopy()
                {
                    return '\n' +
                           "class " + className + '\n' +
                           "{\n" +
                           "private:\n" +
                           "public:\n" +
                           className + "();\n" +
                           className + "(const " + className + "& other);\n" +
                           className + "& operator=(const " + className + "& other);\n" +
                           '~' + className + "();\n" +
                           '}';
                }

                public static string classWithMove()
                {
                    return '\n' +
                           "class " + className + '\n' +
                           "{\n" +
                           "private:\n" +
                           "public:\n" +
                           className + "();\n" +
                           className + "(const " + className + "& other);\n" +
                           className + "(" + className + "&& other);\n" +
                           className + "& operator=(const " + className + "& other);\n" +
                           className + "& operator=(" + className + "&& other);\n" +
                           '~' + className + "();\n" +
                           '}';
                }
            }
            
            public static class ClassDefinitionTemplates
            {
                public static string classBasic()
                {
                    return '\n' +
                           className + "::" + className + "() {}\n" +
                           className + "::" + '~' + className + "() {}\n";
                }

                public static string classWithCopy()
                {
                    return '\n' +
                            className + "::" + className + "() {}\n" +
                            className + "::" + className + "(const " + className +  "& other) {}\n" +
                            className + "::" + className + "& operator=(const " + className +   "& other) {}\n" +
                            className + "::" + className + '~' + className + "() {}\n";
                }

                public static string classWithMove()
                {
                    return '\n' +
                           className + "::" + className + "() {}\n" +
                           className + "::" + className + "(const " + className + "& other) {}\n" +
                           className + "::" + className + "(" + className + "&& other) {}\n" +
                           className + "::" + className + "& operator=(const " + className + "& other) {}\n" +
                           className + "::" + className + "& operator=(" + className + "&& other) {}\n" +
                           className + "::" + className + '~' + className + "() {}\n";
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
        public static void initializeMembers(string className)
        {
            ClassTemplateWriterMembers.className = className;
            ClassTemplateWriterMembers.resetSnapshotLength();
        }

        /// <summary>
        /// Initialize the wpfviewCreationListner from 'TextViewCreationListener'
        /// This only pass the view for the creationListener to the static singleton
        /// </summary>
        /// <param name="view"></param>
        internal static void initializeFromParent(IWpfTextView view)
        {
            ClassTemplateWriterMembers.view = view; //the view for the editor
        }

        public static class ClassDeclarationTemplates
        {
            public static void InsertClassBasic()
            {
                ClassTemplateWriterMembers.edit.Insert(ClassTemplateWriterMembers.Codelength,
                    ClassTemplateWriterMembers.ClassDeclarationTemplates.classBasic());
                ClassTemplateWriterMembers.edit.Apply();
            }

            public static void InsertClassWithCopy()
            {
                ClassTemplateWriterMembers.edit.Insert(ClassTemplateWriterMembers.Codelength,
                    ClassTemplateWriterMembers.ClassDeclarationTemplates.classWithCopy());
                ClassTemplateWriterMembers.edit.Apply();
            }

            public static void InsertClassWithMove()
            {
                ClassTemplateWriterMembers.edit.Insert(ClassTemplateWriterMembers.Codelength,
                    ClassTemplateWriterMembers.ClassDeclarationTemplates.classWithMove());
                ClassTemplateWriterMembers.edit.Apply();
            }
        }

        public static class ClassDefinitionTemplates
        {
            public static void InsertClassBasic()
            {
                ClassTemplateWriterMembers.edit.Insert(ClassTemplateWriterMembers.Codelength,
                    ClassTemplateWriterMembers.ClassDefinitionTemplates.classBasic());
                ClassTemplateWriterMembers.edit.Apply();
            }

            public static void InsertClassWithCopy()
            {
                ClassTemplateWriterMembers.edit.Insert(ClassTemplateWriterMembers.Codelength,
                    ClassTemplateWriterMembers.ClassDefinitionTemplates.classWithCopy());
                ClassTemplateWriterMembers.edit.Apply();
            }

            public static void InsertClassWithMove()
            {
                ClassTemplateWriterMembers.edit.Insert(ClassTemplateWriterMembers.Codelength,
                    ClassTemplateWriterMembers.ClassDefinitionTemplates.classWithMove());
                ClassTemplateWriterMembers.edit.Apply();
            }
        }
    }
}
