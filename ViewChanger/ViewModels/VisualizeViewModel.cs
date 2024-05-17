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
using System.Printing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SymetricBlockEncrypter.ViewModels
{
    public class VisualizeViewModel : INotifyPropertyChanged
    {
        #region Consturctors
        public VisualizeViewModel()
        {
            // Paths of default images to show
            this._rootFolder = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\..\..\..";

            this._originalImage = _rootFolder + @"\Assets\Images\Obama.bmp";
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
            this._selectedDecryptionType = this._encryptionTypes[0];

            // Set up encryption and decryption buttons
            this.EncryptImageCommand = new RelayCommand(() => {
                this.EncryptImage();
                this.SelectedDecryptionType = this._selectedEncryptionType;
                this.PixelXCoordinate = "0";
                this.PixelYCoordinate = "0";
            });
            this.DecryptImageCommand = new RelayCommand(() => {
                this.InitVectorModifiedValue = this._initVectorModifiedValue.PadLeft(this._initVectorOriginalValue.Length, '0');
                RaisePropertyChanged(nameof(InitVectorModifiedValue));
                this.DecryptImage();
            });

            // Initialize AES
            this._aesEncryptor = new AESEncryption();
            this._vectorIV = "";

            //set up command for pixel change
            this.OverwriteImageCommand = new RelayCommand(OverwriteImage);

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

        #endregion


        #region Members

        // Image members
        private string _originalImage;
        private ImageSource _encryptedImage;
        private ImageSource _decryptedImage;

        // Drop down menu members
        private ObservableCollection<string> _encryptionTypes;
        private string _selectedEncryptionType;
        private string _selectedDecryptionType;

        // Init vector members
        private string _initVectorOriginalValue;
        private string _initVectorModifiedValue;

        // Encrpytion members
        private AESEncryption _aesEncryptor;
        private string _vectorIV;

        //image altering members
        private string _pixelXCoordinate = "0";
        private string _pixelYCoordinate = "0";
        private string _pixelRedValue = "0";
        private string _pixelGreenValue = "0";
        private string _pixelBlueValue = "0";
        private static int PIXEL_COLOR_MAX_VALUE = 255;

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
                    RaisePropertyChanged(nameof(OriginalImage));
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
                    RaisePropertyChanged(nameof(SelectedEncryptionType));
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
                    RaisePropertyChanged(nameof(SelectedDecryptionType));
                }
            }
        }

        // Encrypt & Decrypt Buttons Properties
        public ICommand EncryptImageCommand { get; }
        public ICommand DecryptImageCommand { get; }

        // Pixel modification properties
        public string PixelXCoordinate
        {
            get { return this._pixelXCoordinate; }
            set
            {
                if (value != this._pixelXCoordinate && _encryptedImage != null && ValidateInputNumber(value))
                {
                    int x = Int32.Parse(value);
                    if (x >= _encryptedImage.Width)
                    {
                        this._pixelXCoordinate = $"{(int)_encryptedImage.Width - 1}";
                    }
                    else
                    {
                        this._pixelXCoordinate = $"{x}";
                    }
                    RaisePropertyChanged(nameof(PixelXCoordinate));
                }
                else if (value == null || value.Length == 0)
                {
                    this._pixelXCoordinate = "0";
                    RaisePropertyChanged(nameof(PixelXCoordinate));
                }
            }
        }
        public string PixelYCoordinate
        {
            get { return this._pixelYCoordinate; }
            set
            {
                if (value != this._pixelYCoordinate && _encryptedImage != null && ValidateInputNumber(value))
                {
                    int y = Int32.Parse(value);
                    if (y >= _encryptedImage.Height)
                    {
                        this._pixelYCoordinate = $"{(int)_encryptedImage.Height - 1}";
                    }
                    else
                    {
                        this._pixelYCoordinate = $"{y}";
                    }
                    RaisePropertyChanged(nameof(PixelYCoordinate));
                }
                else if (value == null || value.Length == 0)
                {
                    this._pixelYCoordinate = "0";
                    RaisePropertyChanged(nameof(PixelYCoordinate));
                }
            }
        }
        public string PixelRedValue
        {
            get { return this._pixelRedValue; }
            set
            {
                if (value != this._pixelRedValue && ValidateInputNumber(value))
                {
                    int num = Int32.Parse(value);
                    if (num > PIXEL_COLOR_MAX_VALUE)
                    {
                        this._pixelRedValue = $"{PIXEL_COLOR_MAX_VALUE}";
                    }
                    else
                    {
                        this._pixelRedValue = $"{num}";
                    }
                    RaisePropertyChanged(nameof(PixelRedValue));
                }
                else if (value == null || value.Length == 0)
                {
                    this._pixelRedValue = "0";
                    RaisePropertyChanged(nameof(PixelRedValue));
                }
            }
        }
        public string PixelGreenValue
        {
            get { return this._pixelGreenValue; }
            set
            {
                if (value != this._pixelGreenValue && ValidateInputNumber(value))
                {
                    int num = Int32.Parse(value);
                    if (num > PIXEL_COLOR_MAX_VALUE)
                    {
                        this._pixelGreenValue = $"{PIXEL_COLOR_MAX_VALUE}";
                    }
                    else
                    {
                        this._pixelGreenValue = $"{num}";
                    }
                    RaisePropertyChanged(nameof(PixelGreenValue));
                }
                else if (value == null || value.Length == 0)
                {
                    this._pixelGreenValue = "0";
                    RaisePropertyChanged(nameof(PixelGreenValue));
                }
            }
        }
        public string PixelBlueValue
        {
            get { return this._pixelBlueValue; }
            set
            {
                if (value != this._pixelBlueValue && ValidateInputNumber(value))
                {
                    int num = Int32.Parse(value);
                    if (num > PIXEL_COLOR_MAX_VALUE)
                    {
                        this._pixelBlueValue = $"{PIXEL_COLOR_MAX_VALUE}";
                    }
                    else
                    {
                        this._pixelBlueValue = $"{num}";
                    }
                    RaisePropertyChanged(nameof(PixelBlueValue));
                }
                else if (value == null || value.Length == 0)
                {
                    this._pixelBlueValue = "0";
                    RaisePropertyChanged(nameof(PixelBlueValue));
                }
            }
        }

        // Submit button command
        public ICommand OverwriteImageCommand { get; }

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
                int len = this._initVectorOriginalValue.Length;
                string pattern = $"^[0-9A-Fa-f]{{0,{len}}}$";
                if (value != this._initVectorModifiedValue && Regex.IsMatch(value, pattern))
                {    
                    this._initVectorModifiedValue = value.ToUpper();
                    RaisePropertyChanged(nameof(InitVectorModifiedValue));
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
            // Accept only .bmp file types
            openFileDialog.Filter = "Image Files (*.bmp)|*.bmp|All Files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == true)
            {
                this.OriginalImage = openFileDialog.FileName;
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

            _aesEncryptor.SetEncryptionBlockMode(_selectedDecryptionType);
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
            _aesEncryptor.SetEncryptionBlockMode(_selectedEncryptionType);
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

        // Modifies one selected image
        private void OverwriteImage()
        {
            if (this._encryptedImage != null)
            {
                // Get pixel data
                int x = Int32.Parse(PixelXCoordinate);
                int y = Int32.Parse(PixelYCoordinate);
                y += 1;
                byte red = Byte.Parse(_pixelRedValue);
                byte green = Byte.Parse(_pixelGreenValue);
                byte blue = Byte.Parse(_pixelBlueValue);

                // Get encryptedImage path
                BitmapImage inputImage = _encryptedImage as BitmapImage;
                Uri inputUri = inputImage.UriSource;
                string tmpImagePath = inputUri.AbsolutePath;


                byte[] bytes = System.IO.File.ReadAllBytes(tmpImagePath);

                Bitmap ciphertext = new Bitmap(tmpImagePath);
                int width = ciphertext.Width;
                int height = ciphertext.Height;
                ciphertext.Dispose();

                int bytesPerPixel = 3;

                int index = (y * width + x - 2) * bytesPerPixel;

                if(x >= width || y > height || index <= 0)
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

                EncryptedImage = BitmapFromUri(new Uri(tmpImagePath));
            }
        }

        // Checks if input string is a unsigned integer
        private bool ValidateInputNumber(string s)
        {
            if (s != null)
            {
                if (s.Length > 0)
                {
                    bool isUnsignedIntegerNumber = Regex.IsMatch(s, @"^\d+$");
                    return isUnsignedIntegerNumber;
                }
            }
            return false;
        }

        #endregion
    }

}
