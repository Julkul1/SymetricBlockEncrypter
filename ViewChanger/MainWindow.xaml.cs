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
using SymetricBlockEncrypter.ViewModels;

namespace SymetricBlockEncrypter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Visualize_Clicked(null, null);
        }

        private void Visualize_Clicked(object sender, RoutedEventArgs e)
        {
            this.Width = 1210;
            this.Height = 830;
            this.ResizeMode = ResizeMode.CanResize;
            OrangeNavbarButton.Visibility = Visibility.Visible;
            VisualiseNavbarButton.Visibility = Visibility.Collapsed;
            DataContext = new VisualizeViewModel();
        }

        private void BlueView_Clicked(object sender, RoutedEventArgs e)
        {
            DataContext = new BlueViewModel();
        }

        private void OrangeView_Clicked(object sender, RoutedEventArgs e)
        {
            this.Width = 315;
            this.Height = 260;
            this.ResizeMode = ResizeMode.NoResize;
            OrangeNavbarButton.Visibility = Visibility.Collapsed;
            VisualiseNavbarButton.Visibility = Visibility.Visible;
            DataContext = new OrangeViewModel();
        }
    }
}
