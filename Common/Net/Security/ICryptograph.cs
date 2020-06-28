using System;

namespace NineToFive.Security {
    public interface ICryptograph : IDisposable {
        byte[] Encrypt(byte[] data);
        byte[] Decrypt(byte[] data);
    }
}
