using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using Microsoft.VisualStudio.PlatformUI;
using System.Windows.Controls;
using System.Windows.Input;

namespace FastClassesVSIX
{
    /// <summary>
    /// Interaction logic for ClassPreferenceOptions.xaml.
    /// </summary>
    public partial class ClassPreferenceOptions : DialogWindow
    {
        public string inputClassName;
        public ClassPreferenceOptions()
        {
            InitializeComponent();
        }
        private void ClassNameInputTextBox_OnKeyDown(object sender, KeyEventArgs e) //get the text when enter is pressed.
        {
            if (e.Key == Key.Return)
            {
              //  if(ClassNameInputTextBox.Text == String.Empty)
                    //the exception handling stuff your supposed to do
                inputClassName = ClassNameInputTextBox.Text;

            }
            if (e.Key == Key.Cancel || e.Key == Key.Escape)
            {
            }
        }
    }
}
