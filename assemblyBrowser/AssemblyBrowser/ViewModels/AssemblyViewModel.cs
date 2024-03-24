using AssemblyBrowser.Commands;
using AssemblyBrowser.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AssemblyBrowser.ViewModels
{
    internal class AssemblyViewModel : INotifyPropertyChanged
    {
        private Core.AssemblyBrowser browser;
        private ObservableCollection<BrowserNode> headNodes = new();
        public ObservableCollection<BrowserNode> AssemblyHeads => headNodes;

        public AssemblyViewModel()
        {
            browser = new Core.AssemblyBrowser();
        }

        private RelayCommand? addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                    (addCommand = new RelayCommand(AddNewAssemblyInfo));
            }
        }

        private void AddNewAssemblyInfo(object obj)
        {
            var path = obj as string;
            if (!File.Exists(path))
                MessageBox.Show($"Unable to find asssembly by path {path}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                var assembly = browser.LoadFrom(path);
                headNodes.Add(new AssemblyNode(assembly));
                OnPropertyChanged(nameof(AssemblyHeads));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
