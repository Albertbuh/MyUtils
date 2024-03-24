using AssemblyBrowser.ViewModels;
using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AssemblyBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AssemblyViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();

            viewModel = new AssemblyViewModel();
            DataContext = viewModel;
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.O && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                var path = GetFilePath();
                if (path != null)
                {
                    viewModel.AddCommand.Execute(path);                
                }
            }
        }

        private string? GetFilePath()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "C:\\";
            openFileDialog.Filter = "Dynamic Link Libraries (*.dll)|*.dll|All Files (*.*)|*.*";
            var result = openFileDialog.ShowDialog();
            if (result == true)
                return openFileDialog.FileName;
            return null;
        }
    }
}