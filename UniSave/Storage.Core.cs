using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using UniRx.Async;
using UnityEngine;
using Utf8Json;
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable ForCanBeConvertedToForeach
// ReSharper disable once CheckNamespace

namespace UniSave
{
    public static class Storage<T> where T : struct
    {
        private static string PassWord => "projekt";
        private static string Pass => $"{Application.dataPath}/{Application.productName}/";
        private static Encoding Encoding => Encoding.Unicode;
        private static string End => "=";
        
        #region Public Method
        
        public static void Save(T type,string fileName,bool append = false) => SaveSync(type, fileName, append);
        public static async UniTask SaveAsync(T type,string fileName,bool append = false) => await _SaveAsync(type, fileName, append);
        
        public static void SaveArray(IReadOnlyList<T> array, string fileName, bool append = false) => SaveArraySync(array, fileName, append);
        public static async UniTask SaveArrayAsync(IReadOnlyList<T> array, string fileName, bool append = false) => await _SaveArrayAsync(array, fileName, append);

        public static void Delete(string fileName) => DeleteSync(fileName);
        public static void DeleteAll() => DeleteAllSync();
        
        public static T Load(string fileName) => LoadSync(fileName);
        public static T[] LoadArray(string fileName) => LoadArraySync(fileName);
        public static UniTask<T> LoadAsync(string fileName) => LoadAsynchronous(fileName);
        public static UniTask<IReadOnlyList<T>> LoadArrayAsync(string fileName) => LoadArrayAsynchronous(fileName);
        
        #endregion
        
        #region Private Method

        private static void SaveSync(T type, string fileName,bool append = false)
        {
            var encrypted = EncryptBuilder(type);
            
            WriteSync(encrypted, fileName, append);
        }

        private static async UniTask _SaveAsync(T type, string fileName,bool append = false)
        {
            var encrypted = EncryptBuilder(type);

            await WriteAsync(encrypted, fileName, append);
        }
        private static void SaveArraySync(IReadOnlyList<T> array, string fileName, bool append = false)
        {
            var encryptedArray = new string[array.Count];
            
            for (var i = 0; i < array.Count; i++)
            {
                var type = array[i];
                var encrypted = EncryptBuilder(type);
                encryptedArray[i] = encrypted;
            }

            WriteArraySync(encryptedArray, fileName, append);
        }

        private static async UniTask _SaveArrayAsync(IReadOnlyList<T> array, string fileName, bool append)
        {
            var encryptedArray = new string[array.Count];
            
            for (var i = 0; i < array.Count; i++)
            {
                var type = array[i];
                var encrypted = EncryptBuilder(type);
                encryptedArray[i] = encrypted;
            }

            await WriteArrayAsync(encryptedArray, fileName, append);
        }

        #endregion
        
        #region File Write & Load & Delete

        private static void WriteSync(string encrypt,string fileName,bool append = false)
        {
            CreateOrIgnoreDirectory();
            CreateOrIgnoreFile(fileName);

            using (var stream = new StreamWriter($"{Pass}{fileName}",append))
            {
                stream.Write($"{encrypt}{End}");
            }
        }

        private static async UniTask WriteAsync(string encrypt,string fileName,bool append = false)
        {
            CreateOrIgnoreDirectory();
            CreateOrIgnoreFile(fileName);

            using (var stream = new StreamWriter($"{Pass}{fileName}",append,Encoding))
            {
                await stream.WriteAsync($"{encrypt}{End}");
            }
        }

        private static void WriteArraySync(IReadOnlyList<string> encrypts, string fileName, bool append = false)
        {
            CreateOrIgnoreDirectory();
            CreateOrIgnoreFile(fileName);
            
            using (var stream = new StreamWriter($"{Pass}{fileName}",append,Encoding))
            {
                for (var i = 0; i < encrypts.Count; i++)
                {
                    var encrypt = encrypts[i];
                    stream.WriteLine($"{encrypt}{End}");
                }
            }
        }
        
        private static async UniTask WriteArrayAsync(IReadOnlyList<string> encrypts, string fileName, bool append = false)
        {
            CreateOrIgnoreDirectory();
            CreateOrIgnoreFile(fileName);
            
            using (var stream = new StreamWriter($"{Pass}{fileName}",append,Encoding))
            {
                for (var i = 0; i < encrypts.Count; i++)
                {
                    var encrypt = encrypts[i];
                    await stream.WriteLineAsync($"{encrypt}{End}");
                }
            }
        }
        
        private static T LoadSync(string fileName)
        {
            if (!IsFileExist(fileName)) 
                throw new DirectoryNotFoundException();
            
            using (var sr = new StreamReader($"{Pass}{fileName}", Encoding))
            {
                var encryptedArray = ReadAllLineSync(sr);
                var encrypted = encryptedArray[0];
                var decrypted = DecryptBuilder(encrypted);
                var deserialize = Deserialize(decrypted);
                
                return deserialize;
            }
        }

        private static T[] LoadArraySync(string fileName)
        {
            if(!IsFileExist(fileName))
                throw new DirectoryNotFoundException();
            
            T[] arraySync;
            
            using (var sr = new StreamReader($"{Pass}{fileName}",Encoding))
            {
                var encryptTexts = ReadAllLineSync(sr);
                arraySync = new T[encryptTexts.Count];
                
                for (var i = 0; i < encryptTexts.Count; i++)
                {
                    var encrypt = encryptTexts[i];
                    
                    if(encrypt.Length == 0)
                        continue;
                
                    var decrypted = DecryptBuilder(encrypt);
                    arraySync[i] = Deserialize(decrypted);
                }
            }

            return arraySync;
        }

        private static async UniTask<T> LoadAsynchronous(string fileName)
        {
            if(!IsFileExist(fileName))
                throw new DirectoryNotFoundException();
            
            using (var sr = new StreamReader($"{Pass}{fileName}", Encoding))
            {
                var encryptedList = await ReadAllLineAsync(sr);
                var encrypted = encryptedList[0];
                var decrypted = DecryptBuilder(encrypted);
                var deserialize = Deserialize(decrypted);
                
                return deserialize;
            }
        }
        
        private static async UniTask<IReadOnlyList<T>> LoadArrayAsynchronous(string fileName)
        {
            if(!IsFileExist(fileName))
                throw new DirectoryNotFoundException();
            
            var arrayAsync = new List<T>();
            
            using (var sr = new StreamReader($"{Pass}{fileName}", Encoding))
            {
                foreach (var encrypted in await ReadAllLineAsync(sr))
                {
                    var decrypted = DecryptBuilder(encrypted);
                    var deserialize = Deserialize(decrypted);
                    arrayAsync.Add(deserialize);
                }
            }
            
            return arrayAsync;
        }
        
        private static void DeleteSync(string fileName)
        {
            if(!IsFileExist(fileName))
                return;

            File.Delete($"{Pass}{fileName}");
        }
        
        private static void DeleteAllSync()
        {
            if(!IsDirectoryExist())
                return;

            Directory.Delete(Pass, true);
        }
        
        public static void DeleteFilesWithoutUpdates(uint range)
        {
            var day = DateTime.Now.Day - range;
            var files = Directory.GetFiles(Pass);
            var pass = new List<string>();

            for (var i = 0; i < files.Length; i++)
            {
                var writeDay = File.GetLastWriteTime(files[i]).Day;

                if (day >= writeDay)
                    pass.Add(Path.GetFileName(files[i]));
            }
            
            if(pass.Count == 0)
                return;

            foreach (var file in pass) 
                DeleteSync(file);
        }
        
        public static void Clear(string fileName)
        {
            CreateOrIgnoreDirectory();
            CreateOrIgnoreFile(fileName);
            
            using (var stream = new StreamWriter($"{Pass}{fileName}",false,Encoding))
            {
                stream.Write("");
            }
        }
        
        public static async UniTask ClearAsync(string fileName)
        {
            CreateOrIgnoreDirectory();
            CreateOrIgnoreFile(fileName);
            
            using (var stream = new StreamWriter($"{Pass}{fileName}",false,Encoding)) 
                await stream.WriteAsync("");
        }

        #endregion
        
        #region Builders

        private static string EncryptBuilder(T type)
        {
            var serialize = Serialize(type);
            var encrypt = EncryptBytes(serialize);
            var encrypted = ByteToString(encrypt);

            return encrypted;
        }
       
        private static byte[] DecryptBuilder(string encryptText)
        {
            var encryptBytes = StringToBytes(encryptText);
            var decryptBytes = DecryptBytes(encryptBytes);

            return decryptBytes;
        }
        
        #endregion
        
        #region Utility
        
        public static bool IsFileExist(string fileName) => File.Exists($"{Pass}{fileName}");
        public static bool IsDirectoryExist() => Directory.Exists(Pass);
        
        public static T GetOrDefault(string fileName) =>
            IsFileExist(fileName) ? LoadSync(fileName) : default;
        
        public static T[] GetOrDefault(string fileName, int initializeCount) => 
            IsFileExist(fileName) ? LoadArraySync(fileName) : new T[initializeCount];
        
        public static async UniTask<T> GetOrDefaultAsync(string fileName)=>
            IsFileExist(fileName) ? await LoadAsynchronous(fileName) : default;

        public static async UniTask<IReadOnlyList<T>> GetOrDefaultAsync(string fileName, int initializeCount) => 
            IsFileExist(fileName) ? await LoadArrayAsynchronous(fileName) : new T[initializeCount];
        

        private static byte[] Serialize(T type) => JsonSerializer.Serialize(type);
        private static T Deserialize(byte[] bytes) => JsonSerializer.Deserialize<T>(bytes);

        private static string ByteToString(IReadOnlyList<byte> bytes)
        {
            var length = bytes.Count;
            var array = new char[length];
            
            for (var i = 0; i < length; i++) 
                array[i] = (char) bytes[i];

            return new string(array);
        }
        
        private static byte[] StringToBytes(string encrypt)
        {
            var length = encrypt.Length;
            var array = new byte[length];

            for (var i = 0; i < length; i++) 
                array[i] = (byte) encrypt[i];

            return array;
        }
        
        private static void CreateOrIgnoreFile(string fileName)
        {
            if(IsFileExist(fileName))
                return;
            
            File.Create($"{Pass}{fileName}").Close();
        }
        
        private static void CreateOrIgnoreDirectory()
        {
            if(IsDirectoryExist())
                return;
            
            Directory.CreateDirectory(Pass);
            
            #if !UNITY_STANDALONE
            File.SetAttributes(Pass,FileAttributes.Hidden);
            #endif
        }

        private static List<string> ReadAllLineSync(TextReader sr)
        {
            var value = sr.ReadToEnd();
            var sentences = new List<string>();
            int position;
            var start = 0;
            
            do
            {
                position = value.IndexOf(End, start, StringComparison.Ordinal);
                
                if (position < 0)
                    continue;

                sentences.Add(value.Substring(start, position - start).Trim());
                start = position + 1;
            } while (position > 0);

            return sentences;
        }
        
        private static async UniTask<List<string>> ReadAllLineAsync(TextReader sr)
        {
            var value = await sr.ReadToEndAsync();
            var sentences = new List<string>();
            int position;
            var start = 0;

            do
            {
                position = value.IndexOf(End, start, StringComparison.Ordinal);
                
                if (position < 0)
                    continue;

                sentences.Add(value.Substring(start, position - start).Trim());
                start = position + 1;
            } while (position > 0);

            return sentences;
        }
        
        #endregion
        
        #region Encrypt & Decrypt

        private static byte[] EncryptBytes(byte[] type)
        {
            byte[] encrypt;

            using (var rijndael = new RijndaelManaged())
            {
                GenerateRijndael(rijndael.KeySize,rijndael.BlockSize,out var key,out var iv);

                rijndael.Key = key;
                rijndael.IV = iv;

                encrypt = Encrypt(rijndael, type);
            }

            return encrypt;
        }

        private static byte[] DecryptBytes(byte[] encryptBytes)
        {
            byte[] decrypt;
            
            using (var rijndael = new RijndaelManaged{Padding = PaddingMode.None})
            {
                GenerateRijndael(rijndael.KeySize,rijndael.BlockSize,out var key,out var iv);

                rijndael.Key = key;
                rijndael.IV = iv;

                decrypt = Decrypt(rijndael, encryptBytes);
            }

            return decrypt;
        }
        
        private static byte[] Encrypt(SymmetricAlgorithm symmetricAlgorithm,byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                Debug.LogError("The byte is null or zero length!");
                return bytes;
            }
            
            if (symmetricAlgorithm == null)
            {
                throw new NullReferenceException();
            }
            
            using (var stream = new MemoryStream())
            using (var encryptor = symmetricAlgorithm.CreateEncryptor())
            using (var encrypt = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
            {
                encrypt.Write(bytes, 0, bytes.Length);
                encrypt.FlushFinalBlock();
                
                return stream.ToArray();
            }
        }

        private static byte[] Decrypt(SymmetricAlgorithm symmetricAlgorithm,byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                Debug.LogError("The byte is null or zero length!");
                return bytes;
            }

            if (symmetricAlgorithm == null)
            {
                throw new NullReferenceException();
            }
            
            using (var stream = new MemoryStream())
            using (var decryptor = symmetricAlgorithm.CreateDecryptor())
            using (var decrypt = new CryptoStream(stream, decryptor, CryptoStreamMode.Write))
            {
                decrypt.Write(bytes, 0, bytes.Length);
                decrypt.FlushFinalBlock();
                
                return stream.ToArray();
            }
        }
        
        private static void GenerateRijndael(int keySize, int blockSize, out byte[] key, out byte[] iv)
        {
            var salt = JsonSerializer.Serialize(PassWord);

            var deriveBytes = new Rfc2898DeriveBytes(PassWord, salt)
            {
                IterationCount = 1200
            };

            key = deriveBytes.GetBytes(keySize / 8);
            iv = deriveBytes.GetBytes(blockSize / 8);
        }

        #endregion
    }

}
