using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.PlatformUI;

namespace FastClassesVSIX
{
    /// <summary>
    ///     Interaction logic for ClassPreferenceOptions.xaml.
    /// </summary>
    public partial class ClassNameInputMMDBControl : DialogWindow
    {
        public string InputClassName;
        public bool Result { get; private set; }
        public bool CheckBox { get; private set; }
        public ClassNameInputMMDBControl()
        {
            InitializeComponent();
            HasMinimizeButton = false;
            HasMaximizeButton = false;
            ClassNameInputTextBox.Focusable = true;
            Result = false;
            CheckBox = true;

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
                return false;
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
                Close();
            }
            if ((e.Key == Key.Cancel) || (e.Key == Key.Escape))
            {
                Result = false;
                Close();
            }
        }
        private void CheckBoxAddMemberVariable_OnChecked(object sender, RoutedEventArgs e)
        {
            CheckBox = true;
        }
        private void CheckBoxAddMemberVariable_OnUnchecked(object sender, RoutedEventArgs e)
        {
            CheckBox = false;
        }
    }
}