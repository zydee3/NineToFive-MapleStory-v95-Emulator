using NineToFive;
using NineToFive.IO;
using NineToFive.Util;
using System;
using System.IO;

namespace Destiny.Security {
    public class MapleCryptoHandler : ICryptograph {
        public AesCryptograph En { get; private set; }
        public AesCryptograph De { get; private set; }

        /// <summary>
        /// Generates Initialization Vector keys and returns the handshake packet
        /// </summary>
        public byte[] Initialize() {
            byte[] receiveIV = BitConverter.GetBytes(RNG.GetUInt());
            byte[] sendIV = BitConverter.GetBytes(RNG.GetUInt());

            En = new AesCryptograph(sendIV, unchecked((short)(0xFFFF - Constants.GameVersion)));
            De = new AesCryptograph(receiveIV, Constants.GameVersion);

            using Packet packet = new Packet(16);
            packet.WriteShort(14);
            packet.WriteShort(Constants.GameVersion);
            packet.WriteString("1");
            packet.WriteBytes(receiveIV);
            packet.WriteBytes(sendIV);
            packet.WriteByte(8);
            return packet.Array;
        }

        /// <summary>
        /// generates the packet header and encrypts the packet
        /// </summary>
        /// <returns>the encrypted packet with the generated header prepended</returns>
        public byte[] Encrypt(byte[] data) {
            lock (this) {
                byte[] result = new byte[data.Length];
                Buffer.BlockCopy(data, 0, result, 0, data.Length);

                byte[] header = En.GenerateHeader(result.Length);

                BlurCryptoHandler.Encrypt(result);
                En.Crypt(result);

                using Packet p = new Packet(data.Length + header.Length);
                p.WriteBytes(header);
                p.WriteBytes(result);

                return p.Array;
            }
        }

        /// <summary>
        /// decrypts data that includes the packet header
        /// </summary>
        /// <returns>the decrypted packet (excluding the header)</returns>
        public byte[] Decrypt(byte[] data) {
            lock (this) {
                using Packet p = new Packet(data);
                byte[] header = p.ReadBytes(4);

                if (!De.IsValidPacket(header)) {
                    throw new InvalidDataException("invalid header");
                }
                int length = AesCryptograph.RetrieveLength(header);
                byte[] content = p.ToArray();
                if (content.Length == length) {
                    De.Crypt(content);
                    BlurCryptoHandler.Decrypt(content);
                    return content;
                } else {
                    throw new CryptographyException(string.Format("Packet length not matching ({0} != {1}).", content.Length, length));
                }
            }
        }

        public void Dispose() {
            En.Dispose();
            De.Dispose();
        }
    }
}
