using System.IO;
using Microsoft.Win32;
using System.Windows.Controls;

namespace RemoteConnectionManager.UserControls
{
    public partial class PropertyFileSave : UserControl
    {
        public PropertyFileSave()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.FileName = Path.GetFileName(TextBox_File.Text);
            dialog.Filter = "*.json|*.json";

            if (dialog.ShowDialog() == true)
            {
                TextBox_File.Clear();
                TextBox_File.AppendText(dialog.FileName);
                TextBox_File.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }
    }
}
