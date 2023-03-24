using System.Windows;
using System.Windows.Forms;

namespace CheckFilesApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;
        }

        private void SelectDirectory_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    _viewModel.SelectDirectory = fbd.SelectedPath;
                }
            }
        }

        private void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ScanDirectory().Wait();
        }
    }
}
