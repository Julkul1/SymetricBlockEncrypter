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
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Intrinsics.X86;

namespace SymetricBlockEncrypter.ViewModels
{
    public class VisualizeViewModel : INotifyPropertyChanged
    {
        #region Consturctors
        public VisualizeViewModel()
        {
            // Paths of default images to show
            this._rootFolder = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\..\..\..";

            ClearTmpFiles();

            this._originalImage = _rootFolder + @"\Assets\Images\Obama.bmp";
            this._originalImageSafeName = "Obama.bmp";
            this._encryptedImage = null;
            this._decryptedImage = null;

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

            // Set up encryption and decryption buttons
            this.EncryptImageCommand = new RelayCommand(EncryptImage);
            this.DecryptImageCommand = new RelayCommand(DecryptImage);

            // Initialize AES
            this._aesEncryptor = new AESEncryption();
            this._vectorIV = "";

            //set up command for pixel change
            this.SubmitCommand = new RelayCommand(OnSubmitUpdatePixel);

            _vectorIV += "10101100";
            _vectorIV += "11111110";
            _vectorIV += "10111110";
            _vectorIV += "10000100";
            _vectorIV += "10111101";
            _vectorIV += "00101100";
            _vectorIV += "10101110";
            _vectorIV += "00101110";
            _vectorIV += "10101101";
            _vectorIV += "00101100";
            _vectorIV += "10101110";
            _vectorIV += "00101111";
            _vectorIV += "10111100";
            _vectorIV += "10101110";
            _vectorIV += "11111100";

            _aesEncryptor.SetInitializationVector(_vectorIV);
            _aesEncryptor.SetEncryptionBlockMode(_encryptionTypes[0]);
            _aesEncryptor.SetAESKey("HELLO WORLD");

            // Set up init vector members
            this._initVectorOriginalValue = _aesEncryptor.InitVectorConverter(_vectorIV, true);
            this._initVectorModifiedValue = _aesEncryptor.InitVectorConverter(_vectorIV, true);
        }

        ~VisualizeViewModel()
        {
            ClearTmpFiles();
        }

        #endregion


        #region Members

        // Image members
        private string _originalImage;
        private string _originalImageSafeName;
        private ImageSource _encryptedImage;
        private ImageSource _decryptedImage;

        // Drop down menu members
        private ObservableCollection<string> _encryptionTypes;
        private string _selectedEncryptionType;

        // Init vector members
        private string _initVectorOriginalValue;
        private string _initVectorModifiedValue;

        // Encrpytion members
        private AESEncryption _aesEncryptor;
        private string _vectorIV;
        //image altering members
        private string _X;
        private string _Y;
        private string _R;
        private string _G;
        private string _B;

        private string _rootFolder;

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

        public ImageSource EncryptedImage
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

        public ImageSource DecryptedImage
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
                    _aesEncryptor.SetEncryptionBlockMode(value);
                    RaisePropertyChanged("SelectedEncryptionType");
                }
            }
        }

        // Encrypt & Decrypt Buttons Properties
        public ICommand EncryptImageCommand { get; }
        public ICommand DecryptImageCommand { get; }

        // pixel modification properties
        public string ModX
        {
            get { return this._X; }
            set
            {
                if (value != this._X)
                {
                    this._X = value;
                    RaisePropertyChanged("ModX");
                }
            }
        }

        public string ModY
        {
            get { return this._Y; }
            set
            {
                if (value != this._Y)
                {
                    this._Y = value;
                    RaisePropertyChanged("ModY");
                }
            }
        }
        public string ModR
        {
            get { return this._R; }
            set
            {
                if (value != this._R)
                {
                    this._R = value;
                    RaisePropertyChanged("ModR");
                }
            }
        }

        public string ModG
        {
            get { return this._G; }
            set
            {
                if (value != this._G)
                {
                    this._G = value;
                    RaisePropertyChanged("ModG");
                }
            }
        }

        public string ModB
        {
            get { return this._B; }
            set
            {
                if (value != this._B)
                {
                    this._B = value;
                    RaisePropertyChanged("ModB");
                }
            }
        }

        //submit button command
        public ICommand SubmitCommand { get; }

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
            openFileDialog.Filter = "Image Files (*.bmp)|*.bmp|All Files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == true)
            {
                this.OriginalImage = openFileDialog.FileName;
                _originalImageSafeName = openFileDialog.SafeFileName;
            }
        }

        private void SaveImage(string saveType)
        {
            if (saveType == "Encrypted" && _encryptedImage == null)
            {
                return;
            }
            if (saveType == "Decrypted" && _decryptedImage == null)
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

            if (saveType == "Encrypted")
            {
                BitmapImage image = _encryptedImage as BitmapImage;
                Uri uri = image.UriSource;
                _aesEncryptor.CopyFile(uri.AbsolutePath, filePath);
            }
            else if (saveType == "Decrypted")
            {
                BitmapImage image = _decryptedImage as BitmapImage;
                Uri uri = image.UriSource;
                _aesEncryptor.CopyFile(uri.AbsolutePath, filePath);
            }
        }

        private void EncryptImage()
        {
            string tmpImagePath = _rootFolder + @"\RuntimeResources\Images\TmpEncrypt.bmp";

            _aesEncryptor.SetInitializationVector(_vectorIV);
            _aesEncryptor.SetInputFilePath(_originalImage);
            _aesEncryptor.SetOutputFilePath(tmpImagePath);
            _aesEncryptor.Encrypt();
            _aesEncryptor.SaveFile();
            EncryptedImage = BitmapFromUri(new Uri(tmpImagePath));
        }

        private void DecryptImage()
        {
            if (_encryptedImage == null)
            {
                return;
            }
            
            string tmpImagePath = _rootFolder + @"\RuntimeResources\Images\TmpDecrypt.bmp";

            // Only vector of the original size and hexadecimal values can be accepted
            if (_initVectorModifiedValue.Length == _initVectorOriginalValue.Length)
            {
                string pattern = "^[0-9A-F]+$";
                if (Regex.IsMatch(_initVectorModifiedValue, pattern))
                {
                    _aesEncryptor.SetInitializationVector(_aesEncryptor.InitVectorConverter(_initVectorModifiedValue, false));
                }
            }

            // Get encryptedImage path
            BitmapImage inputImage = _encryptedImage as BitmapImage;
            Uri inputUri = inputImage.UriSource;
            _aesEncryptor.SetInputFilePath(inputUri.AbsolutePath);

            _aesEncryptor.SetOutputFilePath(tmpImagePath);
            _aesEncryptor.Decrypt();
            _aesEncryptor.SaveFile();
            DecryptedImage = BitmapFromUri(new Uri(tmpImagePath));
        }

        // Create ImageSource stored in RAM
        private ImageSource BitmapFromUri(Uri source)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = source;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.EndInit();
            return bitmap;
            
        }

        private void ClearTmpFiles()
        {
            try
            {
                string[] files = Directory.GetFiles(_rootFolder + @"\RuntimeResources\Images");
                foreach (string file in files)
                {
                    File.Delete(file);
                }
            }
            catch { }
        }
        //checks if the string is a positive integer number
        private bool validateData(ref int u, string s)
        {
            if (s != null)
            {
                if (s.Length > 0)
                {
                    bool isUnsignedIntegerNumber = Regex.IsMatch(s, @"^\d+$");
                    u = Int32.Parse(s);
                    return isUnsignedIntegerNumber;
                }
            }
            return false;
        }


        //extract R,G,B,X,Y fields from the form and validate them
        private bool ExtractData(ref int x, ref int y, ref int r, ref int g, ref int b)
        {
            if (!validateData(ref x, _X))
                return false;
            if (!validateData(ref y, _Y))
                return false;
            if (!validateData(ref r, _R))
                return false;
            if (!validateData(ref g, _G))
                return false;
            if (!validateData(ref b, _B))
                return false;
            return true;
        }

        private void OverwriteImage(int x, int y, int r, int g, int b)
        {
            if (this._encryptedImage != null)
            {
                string tmpImagePath = _rootFolder + @"\RuntimeResources\Images\TmpEncrypt\" + _selectedEncryptionType + _originalImageSafeName;


                byte[] bytes = System.IO.File.ReadAllBytes(tmpImagePath);
                byte red = (byte)r;
                byte green = (byte)g;
                byte blue = (byte)b;

                Bitmap ciphertext = new Bitmap(tmpImagePath);
                int width = ciphertext.Width;
                int height = ciphertext.Height;
                ciphertext.Dispose();

                int bytesPerPixel = 3;

                int index = (y * width + x) * bytesPerPixel;

                if(x >= width || y >= height || index <= 0)
                {
                    MessageBox.Show("The selected coordinates do not fit in the image dimensions");
                    return;
                }

                bytes[index] = red;
                bytes[index + 1] = green;
                bytes[index + 2] = blue;


                using (var writer = new BinaryWriter(File.OpenWrite(tmpImagePath)))
                {
                    writer.Write(bytes);
                }

            }
        }

        //submit button event handler
        public void OnSubmitUpdatePixel()
        {
            int x = 0, y = 0, r = 0, g = 0, b = 0;
            if (!ExtractData(ref x, ref y, ref r, ref g, ref b))
                return; //skip the code if the input is invalid

            OverwriteImage(x, y, r, g, b);


        }

        #endregion
    }

}
