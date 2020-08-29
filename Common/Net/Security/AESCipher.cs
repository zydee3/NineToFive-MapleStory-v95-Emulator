using System;
using System.Security.Cryptography;

namespace NineToFive.Net.Security {
    public static class AesCipher {
        private static readonly byte[] UserKey = {
            0x13, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0xB4, 0x00, 0x00, 0x00,
            0x1B, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x33, 0x00, 0x00, 0x00, 0x52, 0x00, 0x00, 0x00
        };

        private static readonly AesManaged Cipher = new AesManaged() {
            KeySize = 256,
            Key = UserKey,
            Mode = CipherMode.ECB
        };

        public static void Transform(byte[] data, uint seqKey) {
            int remaining = data.Length;
            int length = 0x5B0;
            int start = 0;

            var srcExp = new byte[sizeof(int) * 4];
            var bSeqKey = BitConverter.GetBytes(seqKey);

            while (remaining > 0) {
                for (var i = 0; i < srcExp.Length; ++i) {
                    srcExp[i] = bSeqKey[i % 4];
                }

                if (remaining < length) length = remaining;

                for (var i = start; i < start + length; ++i) {
                    var sub = i - start;
                    if (sub % srcExp.Length == 0) {
                        using var crypt = Cipher.CreateEncryptor();
                        var result = crypt.TransformFinalBlock(srcExp, 0, srcExp.Length);
                        Array.Copy(result, srcExp, srcExp.Length);
                    }

                    data[i] ^= srcExp[sub % srcExp.Length];
                }

                start += length;
                remaining -= length;
                length = 0x5B4;
            }
        }
    }
}