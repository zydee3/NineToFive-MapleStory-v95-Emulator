using System;
using System.IO;
using Destiny.Security;
using NineToFive.IO;
using NineToFive.Util;

namespace NineToFive.Security {
    public class MapleCryptoHandler : ICryptograph {
        private AesCryptograph En { get; set; }
        private AesCryptograph De { get; set; }

        /// <summary>
        /// Generates Initialization Vector keys and returns the handshake packet
        /// </summary>
        public byte[] Initialize() {
            byte[] receiveIv = BitConverter.GetBytes(RNG.GetUInt());
            byte[] sendIv = BitConverter.GetBytes(RNG.GetUInt());

            En = new AesCryptograph(sendIv, unchecked((short) (0xFFFF - Constants.Server.GameVersion)));
            De = new AesCryptograph(receiveIv, Constants.Server.GameVersion);

            using Packet packet = new Packet();
            packet.WriteShort(14);
            packet.WriteShort(Constants.Server.GameVersion);
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
                if (!De.IsValidPacket(data)) {
                    throw new InvalidDataException("invalid header");
                }

                int length = AesCryptograph.RetrieveLength(data);

                if ((data.Length - 4) != length)
                    throw new CryptographyException($"Packet length not matching ({data.Length} != {length}).");

                byte[] packet = new byte[data.Length - 4];
                Buffer.BlockCopy(data, 4, packet, 0, packet.Length);
                De.Crypt(packet);
                BlurCryptoHandler.Decrypt(packet);
                return packet;
            }
        }

        public void Dispose() {
            En.Dispose();
            De.Dispose();
        }
    }
}