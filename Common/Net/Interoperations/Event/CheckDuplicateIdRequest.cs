using NineToFive.Net;

namespace NineToFive.Interopation.Event {
    public static class CheckDuplicateIdRequest {
        public static byte[] OnHandle(Packet r) {
            string username = r.ReadString();
            return new byte[] {0}; // todo
        }
    }
}