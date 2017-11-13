using Microsoft.Win32;
using System.Windows.Controls;

namespace RemoteConnectionManager.UserControls
{
    public partial class PropertyFile : UserControl
    {
        public PropertyFile()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = "*.ppk|*.ppk|*.*|*.*";

            if (dialog.ShowDialog() == true)
            {
                TextBox_File.Clear();
                TextBox_File.AppendText(dialog.FileName);
                TextBox_File.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }
    }
}
