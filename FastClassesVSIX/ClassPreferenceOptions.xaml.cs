﻿using System;
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
        public bool result;
        public ClassPreferenceOptions()
        {
            InitializeComponent();
            this.HasMinimizeButton = false;
            this.HasMaximizeButton = false;
            result = false;
        }
        private void ClassNameInputTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //  if(ClassNameInputTextBox.Text == String.Empty)
                //the exception handling stuff your supposed to do
                inputClassName = ClassNameInputTextBox.Text;
                result = true;
                this.Close();
            }
            if (e.Key == Key.Cancel || e.Key == Key.Escape)
            {
                result = false;
                this.Close();
            }
        }
    }
}
