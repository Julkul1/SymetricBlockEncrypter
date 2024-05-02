using Microsoft.Win32;
using SymetricBlockEncrypter.Commands;
using SymetricBlockEncrypter.Models;
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

            this._fileName = "Select a file";
            this._filePath = string.Empty;


            // Set up Commands for file Download and Upload buttons
            this.SelectFileCommand = new RelayCommand(SelectFile);
            this.EncryptFileCommand = new RelayCommand(() => SaveFile("Encrypt"));
            this.DecryptFileCommand = new RelayCommand(() => SaveFile("Decrypt"));

            // Set up drop down menu to choose encryption modes from
            this._encryptionTypes = new ObservableCollection<string>()
            {
                "ECB", "CBC", "CFB", "CTR"
            };
            this._selectedEncryptionType = this._encryptionTypes[0];

            // Initialize AES
            this.aesEncryptor = new AESEncryption();
            this.IV = "";

            IV += "10101100";
            IV += "11111110";
            IV += "10111110";
            IV += "10000100";
            IV += "10111101";
            IV += "00101100";
            IV += "10101110";
            IV += "00101110";
            IV += "10101101";
            IV += "00101100";
            IV += "10101110";
            IV += "00101111";
            IV += "10111100";
            IV += "10101110";
            IV += "11111100";

            aesEncryptor.SetInitializationVector(IV);
            aesEncryptor.SetEncryptionBlockMode(_encryptionTypes[0]);
            aesEncryptor.SetAESKey("HELLO WORLD");
        }

        #endregion


        #region Members

        // File name members
        private string _fileName;
        private string _filePath;

        // Drop down menu members
        private ObservableCollection<string> _encryptionTypes;
        private string _selectedEncryptionType;

        // Encrpytion members
        AESEncryption aesEncryptor;
        string IV;

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
                    aesEncryptor.SetEncryptionBlockMode(value);
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

        private void SelectFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All Files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == true)
            {
                this.FileName = openFileDialog.SafeFileName;
                this._filePath = openFileDialog.FileName;
                aesEncryptor.SetInputFilePath(this._filePath);
            }
        }

        private void SaveFile(string saveType)
        {
            if (this._filePath.Equals(string.Empty))
            { 
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "All Files (*.*)|*.*"; // Filter to show all file types
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // Set initial directory

            // Show the dialog and get the result
            bool? result = saveFileDialog.ShowDialog();

            // Process the result
            if (result == false) { return; }

            string filePath = saveFileDialog.FileName;
            // Now you can use filePath to save your downloaded file

            if (saveType == "Encrypt")
            {
                aesEncryptor.SetOutputFilePath(filePath);
                aesEncryptor.Encrypt();
                aesEncryptor.SaveFile();
            }
            else if (saveType == "Decrypt")
            {
                aesEncryptor.SetOutputFilePath(filePath);
                aesEncryptor.Decrypt();
                aesEncryptor.SaveFile();
            }
        }

        #endregion
    }
}

