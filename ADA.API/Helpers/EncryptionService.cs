using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;
using ADAClassLibrary;
using Dapper;
using System.Data;
using ADA.API.DBManager;

namespace ADA.API.Helpers
{
    public class EncryptionService
    {
        private readonly string _key;
        private readonly string _iv;
        private readonly IDapper _dapper;

        public EncryptionService(string key, string iv, IDapper dapper)
        {
            _key = key;
            _iv = iv;
            _dapper = dapper;
        }

        public string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(_key);
                aesAlg.IV = Encoding.UTF8.GetBytes(_iv);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using MemoryStream msEncrypt = new();
                using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
                using StreamWriter swEncrypt = new(csEncrypt);

                swEncrypt.Write(plainText);
                swEncrypt.Flush();
                csEncrypt.FlushFinalBlock();

                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }

        public string Decrypt(string cipherText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(_key);
                aesAlg.IV = Encoding.UTF8.GetBytes(_iv);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using MemoryStream msDecrypt = new(Convert.FromBase64String(cipherText));
                using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
                using StreamReader srDecrypt = new(csDecrypt);

                return srDecrypt.ReadToEnd();
            }
        }

        public object EncryptPassword(Passwords obj)
        {


            DynamicParameters parameters2 = new DynamicParameters();


            parameters2.Add("@Id", obj.Id, DbType.Int32, ParameterDirection.Input);
            parameters2.Add("@Password", obj.Password, DbType.String, ParameterDirection.Input);

            var data = _dapper.Update<int>(@"[dbo].[usp_EncryptPassword]", parameters2);

            return data;
        }
        public object DecryptPassword(Passwords obj)
        {


            DynamicParameters parameters2 = new DynamicParameters();


            parameters2.Add("@Id", obj.Id, DbType.Int32, ParameterDirection.Input);
            parameters2.Add("@Password", obj.Password, DbType.String, ParameterDirection.Input);

            var data = _dapper.Update<int>(@"[dbo].[usp_EncryptPassword]", parameters2);

            return data;
        }
    }
}
