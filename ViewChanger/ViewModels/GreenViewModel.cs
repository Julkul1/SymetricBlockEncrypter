using Microsoft.Win32;
using SymetricBlockEncrypter.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Input;

namespace SymetricBlockEncrypter.ViewModels
{
    public class GreenViewModel : INotifyPropertyChanged
    {
        #region Consturctors
        public GreenViewModel()
        {
            // Paths of default images to show
            string rootFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\..\..";

            this._fileName = "Obama.jpg";
            this._filePath = rootFolder + @"\Assets\Images\Obama.jpg";


            // Set up Commands for Image Download and Upload buttons
            this.SelectFileCommand = new RelayCommand(SelectImage);
            this.EncryptFileCommand = new RelayCommand(() => SaveImage("Encrypt"));
            this.DecryptFileCommand = new RelayCommand(() => SaveImage("Decrypt"));

            // Set up drop down menu to choose encryption modes from
            this._encryptionTypes = new ObservableCollection<string>()
            {
                "ECB", "CBC", "CFB", "CTR"
            };
            this._selectedEncryptionType = this._encryptionTypes[0];

        }

        #endregion


        #region Members

        // File name member
        private string _fileName;
        private string _filePath;

        // Drop down menu members
        private ObservableCollection<string> _encryptionTypes;
        private string _selectedEncryptionType;

        #endregion


        #region Properties

        public string FileName
        {
            get { return this._fileName; }
            set
            {
                if (value != this._fileName)
                {
                    this._fileName = value;
                    RaisePropertyChanged("FileName");
                }
            }
        }

        // Buttons properties
        public ICommand SelectFileCommand { get; }
        public ICommand EncryptFileCommand { get; }
        public ICommand DecryptFileCommand { get; }

        // Drop down menu properties
        public ObservableCollection<string> EncryptionTypes
        {
            get { return this._encryptionTypes; }
        }

        public string SelectedEncryptionType
        {
            get { return "tryb: " + this._selectedEncryptionType; }
            set
            {
                if (value != this._selectedEncryptionType)
                {
                    this._selectedEncryptionType = value;
                    // TODO call function
                }
            }
        }


        #endregion


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        #region Methods

        private void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void SelectImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // Accept only .png .jpg .bmp file types
            openFileDialog.Filter = "Image Files (*.png;*.jpg;*.bmp)|*.png;*.jpg;*.bmp|All Files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == true)
            {
                this.FileName = openFileDialog.SafeFileName;
                this._filePath = openFileDialog.FileName;
            }
        }

        private void SaveImage(string saveType)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "All Files (*.*)|*.*"; // Filter to show all file types
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // Set initial directory

            // Show the dialog and get the result
            bool? result = saveFileDialog.ShowDialog();

            // Process the result
            if (result == false) { return; }

            string filePath = saveFileDialog.FileName;
            // Now you can use filePath to save your downloaded file

            if (saveType == "Encrypted")
            {
                // TODO: Implement file encryption
            }
            else if (saveType == "Decrypted")
            {
                // TODO: Implement file encryption
            }
        }

        #endregion
    }
}

