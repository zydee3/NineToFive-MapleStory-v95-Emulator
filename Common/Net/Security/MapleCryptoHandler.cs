using System;
using Destiny.Security;
using NineToFive.Net;
using NineToFive.Util;

namespace NineToFive.Security {
    public class MapleCryptoHandler : ICryptograph {
        public AesCryptograph En { get; set; }
        public AesCryptograph De { get; set; }

        /// <summary>
        /// Generates Initialization Vector keys and returns the handshake packet
        /// </summary>
        public byte[] Initialize() {
            byte[] receiveIv = BitConverter.GetBytes(RNG.GetUInt());
            byte[] sendIv = BitConverter.GetBytes(RNG.GetUInt());

            En = new AesCryptograph(sendIv, unchecked((short) (0xFFFF - ServerConstants.GameVersion)));
            De = new AesCryptograph(receiveIv, ServerConstants.GameVersion);

            using Packet packet = new Packet();
            packet.WriteShort(14);
            packet.WriteShort(ServerConstants.GameVersion);
            packet.WriteString("1");
            packet.WriteBytes(receiveIv);
            packet.WriteBytes(sendIv);
            packet.WriteByte(8);
            return packet.ToArray();
        }

        /// <summary>
        /// generates the packet header and encrypts the packet
        /// </summary>
        /// <returns>the encrypted packet with the generated header prepended</returns>
        public byte[] Encrypt(byte[] data) {
            lock (this) {
                byte[] header = En.GenerateHeader(data.Length);
                byte[] packet = new byte[data.Length];
                Buffer.BlockCopy(data, 0, packet, 0, data.Length);
                BlurCryptoHandler.Encrypt(packet);
                En.Crypt(packet);
                byte[] result = new byte[header.Length + data.Length];
                Buffer.BlockCopy(header, 0, result, 0, header.Length);
                Buffer.BlockCopy(packet, 0, result, header.Length, packet.Length);
                return result;
            }
        }

        /// <summary>
        /// decrypts data that includes the packet header
        /// </summary>
        /// <returns>the decrypted packet (excluding the header)</returns>
        public byte[] Decrypt(byte[] data) {
            lock (this) {
                De.Crypt(data);
                BlurCryptoHandler.Decrypt(data);
                return data;
            }
        }

        public void Dispose() {
            En.Dispose();
            De.Dispose();
        }
    }
}