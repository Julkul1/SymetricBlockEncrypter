using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing.IndexedProperties;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using System.Reflection.Metadata.Ecma335;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Windows.Controls;
using System.Collections;
using System.Drawing;



namespace SymetricBlockEncrypter.Models
{

    /*
    [JAN ROGOWSKI 193315]
    The following class implements file encryption using AES algorithm.
    Public methods of this class should be called upon in the specific event handlers
    created during development of application's front-end.
    Please refrain from changing access modifiers if possible
    and use Discord server to ask for further clarification if necessary.     
     */
    internal sealed class AESEncryption
    {

        //constants
        private const double BITS_IN_A_BYTE = 8.0;
        private const int PNG_HEADER_SIZE = 16;

        //fields
        private byte[]? inputFileBinary;
        private byte[]? outputFileBinary;
        private string? inputFilePath;
        private string? outputFilePath;
        AesManaged AES;
        private bool isCounterModeActive = false;


        public AESEncryption()
        {
            AES = new AesManaged();
            AES.Padding = PaddingMode.Zeros;
        }

        // sets an AES encryption key

        public void SetAESKey(string AESKey)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(AESKey);
            byte[] paddedBytes = PadBytes(bytes);
            AES.Key = paddedBytes;
        }

        //sets the FULL (C:\\...) file path for input file
        public void SetInputFilePath(string path)
        {
            this.inputFilePath = path;
        }

        public void SetOutputFilePath(string path)
        {
            this.outputFilePath = path;
        }

        //padding for AES key
        private byte[] PadBytes(byte[] bytes)
        {
            byte[] paddedBytes = new byte[AES.BlockSize / (int)BITS_IN_A_BYTE];
            int numberOfBytes = bytes.Length;
            int padding = (AES.BlockSize / (int)BITS_IN_A_BYTE - numberOfBytes);

            for (int i = padding; i < (AES.BlockSize / 8); i++)
            {
                paddedBytes[i] = bytes[i - padding];
            }
            return paddedBytes;
        }

        //sets the mode of encryption ("ECB", "CBC", "CFB", "CTR")
        public void SetEncryptionBlockMode(string mode)
        {
            switch (mode)
            {
                case "ECB":
                    AES.Mode = CipherMode.ECB;
                    break;
                case "CBC":
                    AES.Mode = CipherMode.CBC;
                    break;
                case "CFB":
                    AES.Mode = CipherMode.CFB;
                    break;
                case "CTR":
                    isCounterModeActive = true;
                    break;
                default:
                    AES.Mode = CipherMode.ECB;
                    break;
            }

        }

        //padding of IV vector, key, and others

        //sets an AES initialization vector
        public void SetInitializationVector(string validatedInitializationVector)
        {
            /*
             * Disclaimer: This function should take a safe string argument
            It does not check for input correctness. The validation should be performed
            before a function call!
            */

            byte[] initializationVector;
            // Calculate the required number of bytes for the initialization vector
            int numberOfBytes = (int)Math.Ceiling(validatedInitializationVector.Length / BITS_IN_A_BYTE);
            // Create a byte array for the initialization vector
            initializationVector = new byte[AES.BlockSize / 8];


            //pad the IV
            int padding = (AES.BlockSize - numberOfBytes * 8) / 8;

            //Populate the IV with the provided data
            for (int i = padding; i < (AES.BlockSize / (int)BITS_IN_A_BYTE); i++)
            {
                int startIndex = (i - padding) * 8;
                int length = Math.Min(8, validatedInitializationVector.Length - startIndex);
                //pad bytes with leading zeroes
                string byteString = validatedInitializationVector.Substring(startIndex, length).PadLeft(8, '0');
                //convert the string to a binary number
                initializationVector[i] = Convert.ToByte(byteString, 2);
            }
            AES.IV = initializationVector;

        }

        //loads the file from a given path into binary
        private bool LoadFile(ref byte[] output, string path)
        {
            if (path != null)
            {
                if (System.IO.File.Exists(path))
                {
                    output = System.IO.File.ReadAllBytes(path);
                    return true;
                }
            }
            return false;
        }

        //call this to save the file to a location of destination
        //will be good for a "save output" button handler
        public void SaveFile()
        {
            using var writer = new BinaryWriter(File.OpenWrite(outputFilePath)); ;
            writer.Write(outputFileBinary);


        }

        private void WriteHeader(MemoryStream inputFileStream, MemoryStream outputFileStream)
        {
            // Read the PNG header bytes (first 8 bytes)
            byte[] header = new byte[PNG_HEADER_SIZE];
            inputFileStream.Read(header, 0, header.Length);

            // Write the header to the output file
            outputFileStream.Write(header, 0, header.Length);
        }



        //encrypts the file using any mode except of CTR
        private void GenericNonCTREncrypt()
        {
            ICryptoTransform encryptor = AES.CreateEncryptor(AES.Key, AES.IV);

            using (MemoryStream inputFileStream = new MemoryStream(inputFileBinary))
            using (MemoryStream outputFileStream = new MemoryStream())
            using (CryptoStream cryptoStream = new CryptoStream(inputFileStream, encryptor, CryptoStreamMode.Read))
            {
                WriteHeader(inputFileStream, outputFileStream);

                // Read and process the rest of the PNG data
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    // Write the decrypted content to the output file
                    outputFileStream.Write(buffer, 0, bytesRead);
                }
                outputFileBinary = outputFileStream.ToArray();

            }
        }

        private void GenericNonCTRDecrypt()
        {


            ICryptoTransform encryptor = AES.CreateDecryptor(AES.Key, AES.IV);

            using (MemoryStream inputFileStream = new MemoryStream(inputFileBinary))
            using (MemoryStream outputFileStream = new MemoryStream())
            using (CryptoStream cryptoStream = new CryptoStream(inputFileStream, encryptor, CryptoStreamMode.Read))
            {
                WriteHeader(inputFileStream, outputFileStream);

                // Read and process the rest of the PNG data
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    // Write the decrypted content to the output file
                    outputFileStream.Write(buffer, 0, bytesRead);
                }
                outputFileBinary = outputFileStream.ToArray();

            }
        }

        // Helper method to increment the counter for CTR mode
        private void IncrementCounter(byte[] counter)
        {
            for (int i = counter.Length - 1; i >= 0; i--)
            {
                if (++counter[i] != 0)
                    break;
            }
        }

        private byte[] InitializeCounter()
        {
            byte[] counter = AES.IV.ToArray(); // Initialize the counter with the IV nonce

            for (int i = (counter.Length) / 2; i < counter.Length; i++)
            {
                counter[i] = 0;
            }

            return counter;
        }

        //encrypts OR decrypts file using CTR mode
        private void EncryptUsingCTR()
        {
            using (MemoryStream inputFileStream = new MemoryStream(inputFileBinary))
            using (MemoryStream outputFileStream = new MemoryStream())
            using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
            {
                // Create AES encryptor in ECB mode


                aesProvider.Key = AES.Key;
                aesProvider.Mode = CipherMode.ECB;

                // Create AES decryptor
                using (ICryptoTransform encryptor = aesProvider.CreateEncryptor(AES.Key, AES.IV))
                {
                    byte[] counter = InitializeCounter();


                    WriteHeader(inputFileStream, outputFileStream);

                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = inputFileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        // Encrypt the counter to generate the keystream
                        byte[] encryptedCounter = encryptor.TransformFinalBlock(counter, 0, counter.Length);

                        // XOR the plaintext data with the keystream to produce the ciphertext
                        for (int i = 0; i < bytesRead; i++)
                        {
                            int keystreamIndex = (i % AES.BlockSize) % encryptedCounter.Length;
                            buffer[i] ^= encryptedCounter[keystreamIndex];
                        }

                        // Write the encrypted data to the output stream
                        outputFileStream.Write(buffer, 0, bytesRead);

                        // Increment the counter for the next block
                        IncrementCounter(counter);
                    }
                }


                // Store the encrypted output
                outputFileBinary = outputFileStream.ToArray();
            }
        }




        // routes file encryption, call this to encrypt pre-loaded file
        public bool Encrypt()
        {
            bool loadExitStatus = LoadFile(ref inputFileBinary, inputFilePath);
            if (loadExitStatus == false)
                return loadExitStatus;

            if (isCounterModeActive == false)
            {
                //ALL THE OTHER MODES GO HERE
                GenericNonCTREncrypt();
            }
            else
            {
                //ENCRYPT USING CTR MODE
                EncryptUsingCTR();
            }

            return true;
        }

        public bool Decrypt()
        {
            bool loadExitStatus = LoadFile(ref inputFileBinary, inputFilePath);
            if (loadExitStatus == false)
                return loadExitStatus;

            if (isCounterModeActive == false)
            {
                //ALL THE OTHER MODES GO HERE
                GenericNonCTRDecrypt();
            }
            else
            {
                //DECRYPT USING CTR MODE
                //WE USE ENCRYPTION BLOCKS AS MICHAEL HOEFT SUGGESTED DURING LECTURE
                EncryptUsingCTR();
            }

            return true;
        }


    }
}
