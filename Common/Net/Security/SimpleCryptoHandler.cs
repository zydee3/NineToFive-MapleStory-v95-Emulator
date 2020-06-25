namespace Destiny.Security {
    public class SimpleCryptoHandler : ICryptograph {
        public byte[] Encrypt(byte[] data) {
            return data;
        }

        public byte[] Decrypt(byte[] data) {
            return data;
        }

        public void Dispose() {
            throw new System.NotImplementedException();
        }
    }
}
