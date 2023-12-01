using System;
using System.Security.Cryptography;

namespace Core
{
    public static class IdGenerator
    {
        public static ulong GenerateId()
        {
            byte[] bytes = new byte[8];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return BitConverter.ToUInt64(bytes, 0);
        }
    }
}