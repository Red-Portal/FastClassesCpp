using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio.PlatformUI;
using System.Windows.Controls;
using System.Windows.Input;

namespace FastClassesVSIX
{
    /// <summary>
    /// Interaction logic for ClassPreferenceOptions.xaml.
    /// </summary>
    public partial class FastClassesModalMessageDialogBoxControl : DialogWindow
    {
        
        public string InputClassName;
        public bool Result;
        public FastClassesModalMessageDialogBoxControl()
        {
            InitializeComponent();
            this.HasMinimizeButton = false;
            this.HasMaximizeButton = false;
            Result = false;

            ClassNameInputTextBox.Focusable = true;
            ClassNameInputTextBox.Focus();
        }

        private bool CheckClassNameAvailability(string inputClassName)
        {
            if (inputClassName.Contains(" "))
            {
                ErrorTextBlock.Text = "Class name cannot contain spaces. Use under scores instead";
                return false;
            }
            if (inputClassName.Length == 0)
            {
                ErrorTextBlock.Text = "Class name cannot be empty. Please enter a class name";
            }
            return true;
        }
        private void ClassNameInputTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //  if(ClassNameInputTextBox.Text == String.Empty)
                //the exception handling stuff your supposed to do
                InputClassName = ClassNameInputTextBox.Text;
                if (!CheckClassNameAvailability(InputClassName))
                    return;

                Result = true;
                this.Close();
            }
            if (e.Key == Key.Cancel || e.Key == Key.Escape)
            {
                Result = false;
                this.Close();
            }
        }
    }
}
