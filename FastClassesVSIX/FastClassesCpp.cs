﻿//------------------------------------------------------------------------------
// <copyright file="FastClassesCpp.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace FastClassesVSIX
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class FastClassesCpp
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int makeClassOption1 = 0x0100;

        public const int makeClassOption2 = 0x0101;
        public const int makeClassOption3 = 0x0102;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("0d389696-59eb-416b-ae57-e8ca5a691498");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="FastClassesCpp"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private FastClassesCpp(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService =
                this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var makeClassOption1_cmdID = new CommandID(CommandSet, makeClassOption1);
                var makeClassOption1_menuItem = new MenuCommand(this.MenuItemCallback, makeClassOption1_cmdID);
                commandService.AddCommand(makeClassOption1_menuItem);

                var makeClassOption2_cmdID = new CommandID(CommandSet, makeClassOption2);
                var makeClassOption2_menuItem = new MenuCommand(this.MenuItemCallback, makeClassOption2_cmdID);
                commandService.AddCommand(makeClassOption2_menuItem);

                var makeClassOption3_cmdID = new CommandID(CommandSet, makeClassOption3);
                var makeClassOption3_menuItem = new MenuCommand(this.MenuItemCallback, makeClassOption3_cmdID);
                commandService.AddCommand(makeClassOption3_menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static FastClassesCpp Instance { get; private set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get { return this.package; }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new FastClassesCpp(package);
        }

        /// <summary>
        /// GetSelected Files
        /// </summary>
        /// <param name="dte"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetSelectedFiles(DTE2 dte)
        {
            var items = (Array) dte.ToolWindows.SolutionExplorer.SelectedItems;

            return from item in items.Cast<UIHierarchyItem>()
                let pi = item.Object as ProjectItem
                select pi.FileNames[1];
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dte"></param>
        /// <param name="definitionFile"></param>
        /// <param name="headerFile"></param>
        /// <returns></returns>
        public bool SelectedFileState(DTE2 dte, out string definitionFile, out string headerFile)
        {
            var selectedFiles = GetSelectedFiles(dte);

                definitionFile = null;
                headerFile = null;
           
            var selectedFilesList = selectedFiles as IList<string> ?? selectedFiles.ToList();
            if (selectedFilesList.Count() > 2)
            {
                MessageBox.Show("error only maximum two files can be selected");
                return false;
            }
            else if (selectedFilesList.Count() == 2)
            {
                if (selectedFilesList.ElementAt(0).Contains(".hpp") || selectedFilesList.ElementAt(0).Contains(".h"))
                {
                    if (selectedFilesList.ElementAt(1).Contains(".cpp"))
                    {
                        definitionFile = selectedFilesList.ElementAt(0);
                        headerFile = selectedFilesList.ElementAt(1);
                        return true;
                    }
                    else
                    {
                        MessageBox.Show(
                            "error please select a .cpp file for the definition, and a .h/.hpp file for the declaration");
                        return false;
                    }
                }
                else if (selectedFilesList.ElementAt(0).Contains(".pp"))
                {
                    if (selectedFilesList.ElementAt(1).Contains(".hpp") || selectedFilesList.ElementAt(1).Contains(".h"))
                    {
                        definitionFile = selectedFilesList.ElementAt(1);
                        headerFile = selectedFilesList.ElementAt(0);
                        return true;
                    }
                    else
                    {
                        MessageBox.Show(
                            "error please select a .cpp file for the definition, and a .h/.hpp file for the declaration");
                        return false;
                    }
                }
            }
            else if (selectedFilesList.Count() == 1)
            {
                if (selectedFilesList.ElementAt(0).Contains(".cpp"))
                {
                    definitionFile = selectedFilesList.ElementAt(0);
                    return true;
                }
                else if (selectedFilesList.ElementAt(0).Contains(".h") || selectedFilesList.ElementAt(0).Contains(".hpp"))
                {
                    headerFile = selectedFilesList.ElementAt(0);
                    return true;
                }
                else
                {
                    MessageBox.Show("error please select a .cpp file for definition or a .h/.hpp file declaration");
                    return false;
                }
            }
            else
                MessageBox.Show("error unknown file select state");
            return false;
        }

        /// <summary>
        ///  RETURN TYPE 1 = .cpp file
        ///  RETURN TYPE 2 = .hpp/.h file
        /// </summary>
        /// <param name="dte"></param>
        /// <returns></returns>
        public int getDocumentTypeOneIsDefTwoIsDecl(DTE2 dte)
        {
            if (dte.ActiveDocument.FullName.Contains(".cpp"))
                return 1;
            else if (dte.ActiveDocument.FullName.Contains(".hpp") || dte.ActiveDocument.FullName.Contains(".h"))
                return 2;

            MessageBox.Show("errror invalid file type");

            return 0;
        }

        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            var dte = (DTE2) ServiceProvider.GetService(typeof(DTE));
            var item = (MenuCommand) sender;
            var documentType = getDocumentTypeOneIsDefTwoIsDecl(dte);

            if (documentType == 0) //exception, invalid file type
                return;

            var fastClassesMMBControlInstance = new FastClassesModalMessageDialogBoxControl();
            fastClassesMMBControlInstance.ShowModal();  //Opens the class name input window in the file "ClassPreferenceOptions.xaml"

            if (!fastClassesMMBControlInstance.Result)
            {
                MessageBox.Show("error: could not accept class name");
                return; //Check if the class name was successfully input
            }

            ClassTemplateWriter.initializeMembers(fastClassesMMBControlInstance.InputClassName); //get the className

            if (documentType == 1)
            {
                switch (item.CommandID.ID)
                {
                    default:
                        MessageBox.Show("error: menu command does not match any command guid");
                        break;
                    case makeClassOption1:
                        ClassTemplateWriter.ClassDefinitionTemplates.InsertClassBasic();
                        break;
                    case makeClassOption2:
                        ClassTemplateWriter.ClassDefinitionTemplates.InsertClassWithCopy();
                        break;
                    case makeClassOption3:
                        ClassTemplateWriter.ClassDefinitionTemplates.InsertClassWithMove();
                        break;
                }
            }
            if (documentType == 2)
            {
                switch (item.CommandID.ID)
                {
                    default:
                        MessageBox.Show("error: menu command does not match any command guid");
                        break;
                    case makeClassOption1:
                        ClassTemplateWriter.ClassDeclarationTemplates.InsertClassBasic();
                        break;
                    case makeClassOption2:
                        ClassTemplateWriter.ClassDeclarationTemplates.InsertClassWithCopy();
                        break;
                    case makeClassOption3:
                        ClassTemplateWriter.ClassDeclarationTemplates.InsertClassWithMove();
                        break;
                }
            }
        }
    }
}
