//------------------------------------------------------------------------------
// <copyright file="FastClassesOptionWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Windows.Input;

namespace FastClassesVSIX
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for FastClassesOptionWindowControl.
    /// </summary>
    public partial class FastClassesOptionWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FastClassesOptionWindowControl"/> class.
        /// </summary>
        public FastClassesOptionWindowControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        public string inputClassName;
        public void visualizeMessageBox() //Show the message box when the command is called
        {
            FastClassesOptionsMessageBox.Visibility = System.Windows.Visibility.Visible;
            MessageBox.Show("massage box visualized!");
        }
        public void collapseMessageBox()
        {
            FastClassesOptionsMessageBox.Visibility = System.Windows.Visibility.Collapsed;
        }
        private void ClassNameInputTextBox_OnKeyDown(object sender, KeyEventArgs e) //get the text when enter is pressed.
        {
            if (e.Key == Key.Return)
            {
              //  if(ClassNameInputTextBox.Text == String.Empty)
                    //the exception handling stuff your supposed to do

                inputClassName = ClassNameInputTextBox.Text;

                collapseMessageBox();
            }
            if (e.Key == Key.Cancel || e.Key == Key.Escape)
            {
                collapseMessageBox();
            }
        }
    }
}