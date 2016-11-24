using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualStudio.PlatformUI;

namespace FastClassesVSIX
{
    /// <summary>
    /// Modal Message Dialog box for variable generation options
    /// </summary>
    public partial class MemberVariableGeneratorMMDBControl : DialogWindow
    {
        public bool IsPublicChecked { get; private set; }
        public bool AddGetChecked { get; private set; }
        public bool AddSetChecked { get; private set; }
        public bool AddCopyChecked { get; private set; }
        public bool AddMoveChecked { get; private set; }
        public bool AddCopyImplemenationChecked { get; private set; }
        public bool repeatCheck { get; private set; }

        public MemberVariableGeneratorMMDBControl()
        {
            InitializeComponent();
            HasMinimizeButton = false;
            HasMaximizeButton = false;
            AddGetChecked = false;
            AddSetChecked = false;
            AddCopyChecked = false;
            AddMoveChecked = false;
            AddCopyImplemenationChecked = false;
            repeatCheck = false;
        }

        private void isPublic_OnChecked(object sender, RoutedEventArgs e)
        {
            IsPublicChecked = true;
        }

        private void isPublic_onUnchecked(object sender, RoutedEventArgs e)
        {
            IsPublicChecked = false;
        }

        private void AddGet_OnChecked(object sender, RoutedEventArgs e)
        {
            AddGetChecked = true;
        }

        private void AddGet_OnUnchecked(object sender, RoutedEventArgs e)
        {
            AddGetChecked = false;
        }

        private void AddSet_OnChecked(object sender, RoutedEventArgs e)
        {
            AddSetChecked = true;
        }

        private void AddSet_OnUnchecked(object sender, RoutedEventArgs e)
        {
            AddSetChecked = false;
        }

        private void AddCopySet_OnChecked(object sender, RoutedEventArgs e)
        {
            AddCopyChecked = true;
        }

        private void AddCopySet_OnUnchecked(object sender, RoutedEventArgs e)
        {
            AddCopyChecked = false;
        }

        private void AddMoveSet_OnChecked(object sender, RoutedEventArgs e)
        {
            AddMoveChecked = true;
        }

        private void AddMoveSet_OnUnchecked(object sender, RoutedEventArgs e)
        {
            AddMoveChecked = false;
        }

        private void AddImplementationInConstructor_OnChecked(object sender, RoutedEventArgs e)
        {
            AddCopyImplemenationChecked = true;
        }

        private void AddImplementationInConstructor_OnUnchecked(object sender, RoutedEventArgs e)
        {
            AddCopyImplemenationChecked = false;
        }

        private void AddGet_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddGetChecked = !AddGetChecked;
                this.AddGet.IsChecked = AddGetChecked;
            } 
        }

        private void AddSet_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddSetChecked = !AddSetChecked;
                this.AddSet.IsChecked = AddGetChecked;
            }
        }

        private void isPublic_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                IsPublicChecked = !IsPublicChecked;
                this.isPublic.IsChecked = IsPublicChecked;
            }
        }

        private void AddCopySet_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddCopyChecked = !AddCopyChecked;
                this.AddCopySet.IsChecked = AddCopyChecked;
            }
        }

        private void AddMoveSet_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddMoveChecked = !AddMoveChecked;
                this.AddMoveSet.IsChecked = AddMoveChecked;
            }
        }

        private void AddImplementationInConstructor_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddCopyImplemenationChecked = !AddCopyImplemenationChecked;
                this.AddImplementationInConstructor.IsChecked = AddCopyImplemenationChecked;
            }
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
                repeatCheck = false;
            }
            else if (e.Key == Key.Enter)
            {
                MemberVarDecParser();
            }
        }

        private void MemberVariableDeclarationInput_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MemberVarDecParser();
            }
        }

        private void MemberVarDecParser()
        {
            string inputText = TextBoxMemberVariableDeclarationInput.Text;
            string[] parsedStringCollection = inputText.Split(' ');

            bool isConstant = false;
            bool isInitialized = false;
            bool isArray = false;
            
            if (inputText.Length == 0)
            {
                this.ErrorTextBlock.Text = "Variable declaration is empty. To abort, press Escape";
                this.ErrorTextBlock.Visibility = Visibility.Visible;
            }
            else if (!inputText.Contains(";"))
            {
                this.ErrorTextBlock.Text = "Semicolon(;) is required in the declaration";
                this.ErrorTextBlock.Visibility = Visibility.Visible;
            }
            else if (!inputText.Contains(" "))
            {
                this.ErrorTextBlock.Text =
                    "Type and name of variable must be seperated with a Space";
                this.ErrorTextBlock.Visibility = Visibility.Visible;
            }
            ////////////////////////////////// Exceptions ///////////////////////////////////



            if (inputText.Contains('['))
            {
                isArray = true;
            }
            if(inputText.)
            if (inputText.Contains('{') || inputText.Contains('('))
            {
                isInitialized = true;
            }
            foreach (var parsedString in parsedStringCollection)
            {
                
            }

            repeatCheck = true;
        }
    }
}
