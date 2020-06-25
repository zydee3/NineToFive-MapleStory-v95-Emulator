using System;

namespace Destiny.Security {
    public interface ICryptograph : IDisposable {
        byte[] Encrypt(byte[] data);
        byte[] Decrypt(byte[] data);
    }
}
