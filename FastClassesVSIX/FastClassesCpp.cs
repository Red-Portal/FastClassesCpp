//------------------------------------------------------------------------------
// <copyright file="FastClassesCpp.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
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

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var makeClassOption1_cmdID = new CommandID(CommandSet, makeClassOption1 );
                var makeClassOption1_menuItem = new MenuCommand(this.MenuItemCallback, makeClassOption1_cmdID);
                commandService.AddCommand(makeClassOption1_menuItem);

                var makeClassOption2_cmdID = new CommandID(CommandSet, makeClassOption2);
                var makeClassOption2_menuItem = new MenuCommand(this.MenuItemCallback, makeClassOption2_cmdID);
                commandService.AddCommand(makeClassOption2_menuItem);

                var makeClassOption3_cmdID = new CommandID(CommandSet, makeClassOption3 );
                var makeClassOption3_menuItem = new MenuCommand(this.MenuItemCallback, makeClassOption3_cmdID);
                commandService.AddCommand(makeClassOption3_menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static FastClassesCpp Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Create a MessageBox Controller Instance.
        /// </summary>
        public static ClassPreferenceOptions messageBoxControlInstance
        {
            get;
            private set;
        }


        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new FastClassesCpp(package);
            messageBoxControlInstance = new ClassPreferenceOptions();
        }

     

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            var item = (MenuCommand) sender;

            var ClassPreferenceOptionsInstance = new ClassPreferenceOptions();
            ClassPreferenceOptionsInstance.ShowModal();

            if (!ClassPreferenceOptionsInstance.result)
                return;

            string className = ClassPreferenceOptionsInstance.inputClassName;
            

            switch (item.CommandID.ID)
            {
                case makeClassOption1:
                    ClassFormWriter.fastClassOption1(className);
                    break;
                case makeClassOption2:
                    ClassFormWriter.fastClassOption2(className);
                    break;
                case makeClassOption3:
                    ClassFormWriter.fastClassOption3(className);
                    break;
            } 
        }
    }
}
