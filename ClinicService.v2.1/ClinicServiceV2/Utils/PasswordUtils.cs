﻿using System.Security.Cryptography;
using System;
using System.Text;

namespace ClinicServiceV2.Utils
{
    public static class PasswordUtils
    {
        private const string SecretKey = "TestKey1!";

        public static (string passwordSalt, string passwordHash) CreatePasswordHash(string password)
        {
            // Генерация соли
            byte[] buffer = new byte[16];
            RNGCryptoServiceProvider secureRandom = new RNGCryptoServiceProvider();
            secureRandom.GetBytes(buffer);

            // create hash 
            string passwordSalt = Convert.ToBase64String(buffer);
            string passwordHash = GetPasswordHash(password, passwordSalt);

            // done
            return (passwordSalt, passwordHash);
        }

        public static bool VerifyPassword(string password, string passwordSalt,
            string passwordHash)
        {
            return GetPasswordHash(password, passwordSalt) == passwordHash;
        }

        public static string GetPasswordHash(string password, string passwordSalt)
        {
            // Усложняю пароль и генерирую соль
            password = $"{password}~{passwordSalt}~{SecretKey}";
            byte[] buffer = Encoding.UTF8.GetBytes(password);

            // compute hash 
            SHA512 sha512 = new SHA512Managed();
            byte[] passwordHash = sha512.ComputeHash(buffer);

            // done
            return Convert.ToBase64String(passwordHash);
        }
    }
}
