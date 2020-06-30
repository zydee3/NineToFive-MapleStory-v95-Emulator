using System;
using NineToFive.Security;

namespace NineToFive.Net.Security {
    public class SimpleCrypto : ICryptograph {
        public void Dispose() { }

        /// <summary>
        /// prepends 4 bytes representing the length of the packet buffer 
        /// </summary>
        /// <returns>buffer with length of the packet prepended</returns>
        public byte[] Encrypt(byte[] data) {
            byte[] length = BitConverter.GetBytes(data.Length);
            byte[] packet = new byte[data.Length + length.Length];
            Buffer.BlockCopy(length, 0, packet, 0, length.Length);
            Buffer.BlockCopy(data, 0, packet, length.Length, data.Length);
            return packet;
        }

        public byte[] Decrypt(byte[] data) {
            throw new InvalidOperationException();
        }
    }
}