using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SymetricBlockEncrypter.Commands;
using SymetricBlockEncrypter.Models;

namespace SymetricBlockEncrypter.ViewModels
{
    public class VisualizeViewModel : INotifyPropertyChanged
    {
        #region Consturctors
        public VisualizeViewModel()
        {
            // Paths of default images to show
            string rootFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\..\..\..";

            this._originalImage = rootFolder + @"\Assets\Images\Obama.jpg";
            this._encryptedImage = rootFolder + @"\Assets\Images\Obama.jpg";
            this._decryptedImage = rootFolder + @"\Assets\Images\Obama.jpg";

            // Set up Commands for Image Download and Upload buttons
            this.SelectImageCommand = new RelayCommand(SelectImage);
            this.SaveEncryptedImageCommand = new RelayCommand(() => this.SaveImage("Encrypted"));
            this.SaveDecryptedImageCommand = new RelayCommand(() => this.SaveImage("Decrypted"));

            // Set up drop down menu to choose encryption modes from
            this._encryptionTypes = new ObservableCollection<string>()
            {
                "ECB", "CBC", "CFB", "CTR"
            };
            this._selectedEncryptionType = this._encryptionTypes[0];
            this._selectedDecryptionType = this._encryptionTypes[0];

            // Set up encryption and decryption buttons
            this.EncryptImageCommand = new RelayCommand(EncryptImage);
            this.DecryptImageCommand = new RelayCommand(DecryptImage);

            // Set up init vector members
            this._initVectorOriginalValue = "#FFFFFFFFFFFFF";
            this._initVectorModifiedValue = "";

            this.AESEncryptorTest = new AESEncryption();
        }

        #endregion


        #region Members

        // Image members
        private string _originalImage;
        private string _encryptedImage;
        private string _decryptedImage;

        // Drop down menu members
        private ObservableCollection<string> _encryptionTypes;
        private string _selectedEncryptionType;
        private string _selectedDecryptionType;

        // Init vector members
        private string _initVectorOriginalValue;
        private string _initVectorModifiedValue;

        AESEncryption AESEncryptorTest;

        #endregion


        #region Properties

        // Image Properties
        public string OriginalImage
        {
            get { return this._originalImage; }
            set
            {
                if (value != this._originalImage)
                {
                    this._originalImage = value;
                    RaisePropertyChanged("OriginalImage");
                }
            }
        }

        public string EncryptedImage
        {
            get { return this._encryptedImage; }
            set
            {
                if (value != this._encryptedImage)
                {
                    this._encryptedImage = value;
                    RaisePropertyChanged("EncryptedImage");
                }
            }
        }

        public string DecryptedImage
        {
            get { return this._decryptedImage; }
            set
            {
                if (value != this._decryptedImage)
                {
                    this._decryptedImage = value;
                    RaisePropertyChanged("DecryptedImage");
                }
            }
        }

        // Image buttons properties
        public ICommand SelectImageCommand { get; }

        public ICommand SaveEncryptedImageCommand { get; }

        public ICommand SaveDecryptedImageCommand { get; }

        // Drop down menu properties
        public ObservableCollection<string> EncryptionTypes
        {
            get { return this._encryptionTypes; }
        }

        public string SelectedEncryptionType
        {
            get { return this._selectedEncryptionType; }
            set
            {
                if (value != this._selectedEncryptionType)
                {
                    this._selectedEncryptionType = value;
                    AESEncryptorTest.SetEncryptionBlockMode(value);
                }
            }
        }

        public string SelectedDecryptionType
        {
            get { return this._selectedDecryptionType; }
            set
            {
                if (value != this._selectedDecryptionType)
                {
                    this._selectedDecryptionType = value;
                }
            }
        }

        // Encrypt & Decrypt Buttons Properties
        public ICommand EncryptImageCommand { get; }
        public ICommand DecryptImageCommand { get; }

        // Init Vector Properties

        public string InitVectorOriginalValue
        {
            get { return this._initVectorOriginalValue; }
            set
            {
                if (value != this._initVectorOriginalValue)
                {
                    this._initVectorOriginalValue = value;
                    RaisePropertyChanged("InitVectorOriginalValue");
                }
            }
        }

        public string InitVectorModifiedValue
        {
            get { return this._initVectorModifiedValue; }
            set
            {
                if (value != this._initVectorModifiedValue)
                {
                    this._initVectorModifiedValue = value;
                    RaisePropertyChanged("InitVectorModifiedValue");
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
                this.OriginalImage = openFileDialog.FileName;
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

        private void EncryptImage()
        {

        }

        private void DecryptImage()
        {

        }

        #endregion
    }

}
