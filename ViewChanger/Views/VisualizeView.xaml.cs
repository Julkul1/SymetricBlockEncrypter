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

namespace SymetricBlockEncrypter.Views
{
    /// <summary>
    /// Interaction logic for RedView.xaml
    /// </summary>
    public partial class VisualizeView : UserControl
    {

        #region Constructors
        public VisualizeView()
        {
            InitializeComponent();
            DataContext = new VisualizeViewModel();
            _viewModel = (VisualizeViewModel)DataContext;
        }

        #endregion


        #region Members

        private VisualizeViewModel _viewModel;

        #endregion

    }
}
