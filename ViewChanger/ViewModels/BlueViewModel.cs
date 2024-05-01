using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SymetricBlockEncrypter.Commands;

namespace SymetricBlockEncrypter.ViewModels
{
    public class BlueViewModel : INotifyPropertyChanged
    {
        #region Construction
        public BlueViewModel()
        {
            _ArtistName = "Helga";
            ChangeNameCommand = new RelayCommand(ChangeName);
        }

        #endregion


        #region Members

        private string _ArtistName;
       
        #endregion


        #region Properties
        public string ArtistName
        {
            get { return this._ArtistName; }
            set
            {
                if (_ArtistName != value)
                {
                    _ArtistName = value;
                    RaisePropertyChanged("ArtistName");
                }
            }
        }

        public ICommand ChangeNameCommand { get; }

        #endregion


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        #region Methods

        private void ChangeName()
        {
            ArtistName = "Ryszard";
        }

        private void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }

}
